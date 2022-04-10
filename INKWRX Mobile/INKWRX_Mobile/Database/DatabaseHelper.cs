using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INKWRX_Mobile.Database.Entity;
using SQLite;
using INKWRX_Mobile.Dependencies;

namespace INKWRX_Mobile.Database
{
    public class DatabaseHelper
    {
        public DatabaseHelper(string dbPath)
        {
            this.db = new SQLiteAsyncConnection(dbPath, storeDateTimeAsTicks: true);
            this.db.CreateTableAsync<User>().Wait();
            this.db.CreateTableAsync<AttachedItem>().Wait();
            this.db.CreateTableAsync<Field>().Wait();
            this.db.CreateTableAsync<Folder>().Wait();
            this.db.CreateTableAsync<Form>().Wait();
            this.db.CreateTableAsync<Setting>().Wait();
            this.db.CreateTableAsync<StrokePath>().Wait();
            this.db.CreateTableAsync<StrokePoint>().Wait();
            this.db.CreateTableAsync<PrepopField>().Wait();
            this.db.CreateTableAsync<PrepopForm>().Wait();
            this.db.CreateTableAsync<Transaction>().Wait();
        }

        #region Enums

        public enum FormStatus
        {
            Available = 0,
            RequiresUpdate = 1,

            Deleted = 99
        }

        public enum Status
        {
            Available = 0,
            Parked = 1,
            Pending = 2,
            Sent = 3,
            Autosaved = 4
        }

        public enum AttachmentType
        {
            All = 0,
            Photo = 1
        }

        public enum AttachmentSource
        {
            Device = 0, // ie, Camera
            Gallery = 1
        }

        #endregion

        private SQLiteAsyncConnection db = null;

        /// <summary>
        /// Update InkwrxBaseTable item in the database
        /// </summary>
        /// <typeparam name="T">The table type</typeparam>
        /// <param name="item">The item to update</param>
        /// <returns>Returns the updated item</returns>
        public async Task<T> UpdateItemAsync<T>(T item) where T : InkwrxBaseTable
        {
            await db.UpdateAsync(item);
            return item;
        }

        #region Users

        /// <summary>
        /// Get a user object from the username
        /// </summary>
        /// <param name="username">The username to retrieve</param>
        /// <returns>User object for the specified user, or null if it does not exist</returns>
        public async Task<User> GetUserAsync(string username)
        {
            return await this.db.Table<User>().Where(usr => usr.Username == username.ToLower()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save a new user, or update the password for an existing one
        /// </summary>
        /// <param name="username">The username to be stored</param>
        /// <param name="password">The password to be stored</param>
        /// <returns>Returns the new or updated object for the user</returns>
        public async Task<User> SaveUser(string username, string password)
        {
            var user = await this.GetUserAsync(username);
            if (user == null)
            {
                user = new User
                {
                    Username = username.ToLower(),
                    Password = password
                };
                await this.db.InsertAsync(user);
                return user;
            }
            user.Password = password;
            user = await this.UpdateItemAsync(user);
            return user;
        }

        #endregion

        #region Folders

        public async Task<bool> FixFolders()
        {
            var folders = await this.db.Table<Folder>().Where(x => x.Parent == x.Id).ToListAsync();
            foreach (var folder in folders)
            {
                folder.Parent = -1;
                await this.UpdateItemAsync(folder);
            }
            
            return true;
        }

        /// <summary>
        /// Get sub-folders of the specified folder
        /// </summary>
        /// <param name="user">The user for which to retrieve the sub-folder</param>
        /// <param name="folder">The folder to retrieve sub-folders for</param>
        /// <returns>Returns a list of sub-folders</returns>
        public async Task<List<Folder>> GetFoldersAsync(User user, Folder folder)
        {
            int parentId = folder == null ? -1 : folder.Id;
            return await db.Table<Folder>().Where(f => f.User == user.Id && f.Parent == parentId).ToListAsync();
        }

        /// <summary>
        /// Get the root folders
        /// </summary>
        /// <param name="user">The user for which to retrieve the sub-folders</param>
        /// <returns>Returns a list of folders at the root level</returns>
        public async Task<List<Folder>> GetFoldersAsync(User user)
        {
            return await this.GetFoldersAsync(user, null);
        }

        public async Task<Folder> GetFolderAsync(int folderId)
        {
            return await this.db.Table<Folder>().Where(f => f.Id == folderId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new sub-folder in the specified folder
        /// </summary>
        /// <param name="user">The user for which to store the sub-folder</param>
        /// <param name="name">The name of the sub-folder to be stored</param>
        /// <param name="parent">The folder in which to create the sub-folder</param>
        /// <returns>Returns the new sub-folder</returns>
        public async Task<Folder> CreateFolderAsync(User user, string name, Folder parent)
        {
            var folder = new Folder
            {
                Name = name,
                User = user.Id,
                Parent = parent == null ? -1 : parent.Id
            };
            await this.db.InsertAsync(folder);
            return folder;
        }

        /// <summary>
        /// Creates a new folder at the root level
        /// </summary>
        /// <param name="user">The user for which to store the folder</param>
        /// <param name="name">The name of the folder</param>
        /// <returns>Returns the new folder</returns>
        public async Task<Folder> CreateFolderAsync(User user, string name)
        {
            return await this.CreateFolderAsync(user, name, null);
        }

        /// <summary>
        /// Deletes Folder and all sub-folders, moving all forms in itself or sub-folders to the parent folder, or root.
        /// </summary>
        /// <param name="folder">Folder to be deleted</param>
        /// <returns>Returns boolean to indicate success</returns>
        public async Task<bool> DeleteFolderAsync(Folder folder)
        {
            var user = await this.db.Table<User>().Where(u => u.Id == folder.User).FirstOrDefaultAsync();
            if (user == null)
            {
                //This should not happen - test for "false" in case.
                return false;
            }

            var folders = await this.GetFoldersAsync(user, folder);
          
            foreach (var subFolder in folders)
            {
                if (!(await this.DeleteFolderAsync(subFolder)))
                {
                    return false;
                }
            }
            
            
            var forms = await this.GetFormsAsync(user, folder);
            foreach (var form in forms)
            {
                form.ParentFolder = folder.Parent;
                await this.UpdateItemAsync(form);
            }

            await this.db.DeleteAsync(folder);

            return true;
        }

        #endregion

        #region Forms

        /// <summary>
        /// Retrieves a single form based on form Id
        /// </summary>
        /// <param name="formId">The database Id of the form requested</param>
        /// <returns>Returns a single Form</returns>
        public async Task<Form> GetFormAsync(int formId)
        {
            return await this.db.Table<Form>().Where(f => f.Id == formId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a list of all the forms regardless of the folder
        /// </summary>
        /// <param name="user">The user for which to retrieve the forms</param>
        /// <returns>Returns a list of the forms at the root level</returns>
        public async Task<List<Form>> GetAllFormsAsync(User user)
        {
            var forms = await this.db.Table<Form>().Where(f => f.User == user.Id).ToListAsync();
            return forms.Where(x => x.Status == (int)FormStatus.Available).ToList();
        }

        public async Task<List<Form>> GetAllFormsAllStatusAsync(User user)
        {
            return await this.db.Table<Form>().Where(f => f.User == user.Id).ToListAsync();
        }

        /// <summary>
        /// Add a temporary form item to the database
        /// </summary>
        /// <param name="form">The temporary form item</param>
        /// <returns>Returns the new form item with database Id included.</returns>
        public async Task<Form> AddForm(Form form)
        {
            await this.db.InsertAsync(form);
            return form;
        }

        /// <summary>
        /// Get a list of all the forms for a specified user, from within a sub-folder
        /// </summary>
        /// <param name="user">The user for which to retrieve forms</param>
        /// <param name="folder">The sub-folder from which to retrieve the forms</param>
        /// <returns>Returns a list of the forms in the sub-folder</returns>
        public async Task<List<Form>> GetFormsAsync(User user, Folder folder)
        {
            var folderId = folder == null ? -1 : folder.Id;
            var folders = await this.db.Table<Form>().Where(f => f.ParentFolder == folderId).ToListAsync();
            return folders.Where(f => f.User == user.Id && f.Status == (int)FormStatus.Available).ToList();
        }

        /// <summary>
        /// Get a list of all the forms in the root folder
        /// </summary>
        /// <param name="user">The user for which to retrieve the forms</param>
        /// <returns>Returns a list of the forms at the root level</returns>
        public async Task<List<Form>> GetFormsAsync(User user)
        {
            return await this.GetFormsAsync(user, null);
        }

        /// <summary>
        /// Deletes a Form and all Prepop items for that form
        /// </summary>
        /// <param name="form">The Form to be deleted.</param>
        /// <returns>Returns a bool to indicate success.</returns>
        public async Task<bool> DeleteForm(Form form)
        {
            await DeletePrepopFormsForApp(form);

            await this.db.DeleteAsync(form);
            return true;
        }

        #endregion

        #region Transactions

        public async Task<bool> ClearTransactionData(Transaction transaction)
        {
            var fields = await this.GetFieldsAsync(transaction);
            
            foreach (var field in fields)
            {
                await this.db.DeleteAsync(field);
            }
            await this.DeleteAttachedItems(transaction);
            await this.DeleteAllStrokesAsync(transaction);
            return true;
        }

        public async Task<Transaction> GetTransactionAsync(int transId)
        {
            return await this.db.Table<Transaction>().Where(t => t.Id == transId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all Transactions for the specified User of the specified Status
        /// </summary>
        /// <param name="user">The User to retrieve Transactions for</param>
        /// <param name="status">The desired Status</param>
        /// <returns>Returns a List of Transactions</returns>
        public async Task<List<Transaction>> GetTransactionsAsync(User user, Status status)
        {
            if (user != null)
            {
                return await this.db.Table<Transaction>().Where(t => t.User == user.Id && t.Status == (int)status).ToListAsync();
            }
            else
            {
                return await this.db.Table<Transaction>().Where(t => t.Status == (int)status).ToListAsync();
            }
        }

        /// <summary>
        /// Gets all Transactions of the specified Status for all users
        /// </summary>
        /// <param name="status">The desired Status</param>
        /// <returns>Returns a List of Transactions</returns>
        public async Task<List<Transaction>> GetTransactionsAsync(Status status)
        {
            return await this.GetTransactionsAsync(null, status);
        }

        /// <summary>
        /// Get a list of Transactions of any Status for the specified user, or all users.
        /// </summary>
        /// <param name="user">The User to retrieve Transactions for, or null for all users</param>
        /// <returns>Returns a List of Transactions</returns>
        public async Task<List<Transaction>> GetTransactionsAsync(User user = null)
        {
            if (user != null)
            {
                return await this.db.Table<Transaction>().Where(t => t.User == user.Id).ToListAsync();
            }
            else
            {
                return await this.db.Table<Transaction>().ToListAsync();
            }
        }

        public async Task<Transaction> GetAutosaveFromOriginal(int originalTransactionId)
        {
            return await this.db.Table<Transaction>().Where(t => t.AutosavedParent == originalTransactionId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Delete a Transaction and all associated AttachedItems, Fields and Strokes
        /// </summary>
        /// <param name="trans">The Transaction to be deleted</param>
        /// <returns>Returns a bool to indicate success</returns>
        public async Task<bool> DeleteTransactionAsync(Transaction trans)
        {
            var attachedItems = await this.GetAttachedItemsAsync(trans);
            foreach (var attachedItem in attachedItems)
            {
                await this.db.DeleteAsync(attachedItem);
            }

            var fields = await this.GetFieldsAsync(trans);
            foreach (var field in fields)
            {
                await this.db.DeleteAsync(field);
            }

            var strokes = await this.GetStrokePathsAsync(trans);
            foreach (var stroke in strokes)
            {
                if (!(await this.DeleteStrokePathAsync(stroke)))
                {
                    return false;
                }
            }

			if (trans.PrepopId != -1 && trans.AutosavedParent == -1)
            {
				
                var prepop = await this.db.Table<PrepopForm>().Where(p => p.Id == trans.PrepopId).FirstOrDefaultAsync();
                if (null != prepop)
                {
                    prepop.Status = (int)Status.Available;
                    await this.db.UpdateAsync(prepop);
                }
            }

            await this.db.DeleteAsync(trans);

            return true;
        }

        public async Task<Transaction> AddTransacionAsync(Transaction transaction)
        {
            await this.db.InsertAsync(transaction);
            return transaction;
        }

        /// <summary>
        /// Deletes all forms for any user that were sent more than 30 days ago.
        /// </summary>
        /// <returns>Returns a bool to indicate success.</returns>
        public async Task<bool> DeleteOver30Days()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var toDelete = await this.db.Table<Transaction>().Where(t => t.Status == (int)Status.Sent).ToListAsync();

            foreach (var trans in toDelete.Where(t => t.SentDate.Value.Ticks < thirtyDaysAgo.Ticks))
            {
                if (!(await this.DeleteTransactionAsync(trans)))
                {
                    return false;
                }
            }

            return true;
        }

        #region Fields

        /// <summary>
        /// Gets a list of all the fields for the specified Transaction
        /// </summary>
        /// <param name="trans">The Transaction for which to retrieve fields</param>
        /// <returns>Returns a list of fields</returns>
        public async Task<List<Field>> GetFieldsAsync(Transaction trans)
        {
            return await this.db.Table<Field>().Where(f => f.Transaction == trans.Id).ToListAsync();
        }

        public async Task<Field> AddFieldAsync(Field field)
        {
            await this.db.InsertAsync(field);
            return field;
        }

        public async Task<List<Field>> AddFieldsAsync(List<Field> fields)
        {
            await this.db.RunInTransactionAsync((connection) =>
            {
                connection.InsertAll(fields);
            });
            return fields;
        }

        #endregion

        #region Strokes

        public async Task<StrokePath> AddStrokePathAsync(StrokePath strokePath)
        {
            await this.db.InsertAsync(strokePath);
            return strokePath;
        }

        public async Task<StrokePoint> AddStrokePointAsync(StrokePoint strokePoint)
        {
            await this.db.InsertAsync(strokePoint);
            return strokePoint;
        }

        public async Task<List<StrokePoint>> AddStrokePointsAsync(List<StrokePoint> strokePoints)
        {
            await this.db.RunInTransactionAsync((connection) =>
            {
                connection.InsertAll(strokePoints);
            });
            return strokePoints;
        }

        /// <summary>
        /// Gets a List of StrokePaths for a specified Transaction
        /// </summary>
        /// <param name="trans">The Transaction for which to retrieve StrokePaths</param>
        /// <returns>Returns a List of StrokePaths</returns>
        public async Task<List<StrokePath>> GetStrokePathsAsync(Transaction trans)
        {
            return await this.db.Table<StrokePath>().Where(sp => sp.Transaction == trans.Id).ToListAsync();
        }

        /// <summary>
        /// Gets a List of StrokePoints for the specified StrokePath
        /// </summary>
        /// <param name="path">The StrokePath for which to retrieve StrokePoints</param>
        /// <returns>Returns a list of StrokePoints</returns>
        public async Task<List<StrokePoint>> GetStrokePointsAsync(StrokePath path)
        {
            return await this.db.Table<StrokePoint>().Where(sp => sp.Path == path.Id).ToListAsync();
        }

        /// <summary>
        /// Deletes a specified StrokePath, and all the points associated with it.
        /// </summary>
        /// <param name="path">The StrokePath to delete.</param>
        /// <returns>Returns a bool to indicate success</returns>
        public async Task<bool> DeleteStrokePathAsync(StrokePath path)
        {
            await this.db.ExecuteAsync("DELETE FROM StrokePoints WHERE Path=?", path.Id);
            //var points = await this.GetStrokePointsAsync(path);
            //foreach (var point in points)
            //{
            //    await this.db.DeleteAsync(point);
            //}
            
            await this.db.DeleteAsync(path);
            return true;
        }
                
        /// <summary>
        /// Delete all strokes for a Transaction
        /// </summary>
        /// <param name="trans">The Transaction for which to delete Strokes</param>
        /// <returns>Returns a bool to indicate success.</returns>
        public async Task<bool> DeleteAllStrokesAsync(Transaction trans)
        {
            var paths = await this.GetStrokePathsAsync(trans);
            foreach (var path in paths)
            {
                if (!(await this.DeleteStrokePathAsync(path)))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Attached Items

        public async Task<AttachedItem> AddAttachedItemAsync(AttachedItem attachedItem)
        {
            await this.db.InsertAsync(attachedItem);
            return attachedItem;
        }

        /// <summary>
        /// Gets all AttachedItems for a Transaction
        /// </summary>
        /// <param name="trans">The Transaction for which to get AttachedItems</param>
        /// <param name="attachType">The type of AttachedItem to retrieve. Default Photo</param>
        /// <returns>List of all AttachedItems retrieved</returns>
        public async Task<List<AttachedItem>> GetAttachedItemsAsync(Transaction trans, AttachmentType attachType = AttachmentType.Photo)
        {
            if (attachType == AttachmentType.All)
            {
                return await this.db.Table<AttachedItem>().Where(a => a.Transaction == trans.Id).ToListAsync();
            }
            return await this.db.Table<AttachedItem>().Where(a => a.Transaction == trans.Id  && a.ItemType == (int)attachType).ToListAsync();
        }

        /// <summary>
        /// Deletes Attached items for the specified Transaction
        /// </summary>
        /// <param name="trans">The Transaction for which to delete AttachedItems</param>
        /// <returns>Returns a bool to indicate success.</returns>
        public async Task<bool> DeleteAttachedItems(Transaction trans)
        {
            var items = await this.GetAttachedItemsAsync(trans, AttachmentType.All);
            foreach (var item in items)
            {
                await db.DeleteAsync(item);
            }

            return true;
        }

        #endregion

        #endregion

        #region Prepop

        /// <summary>
        /// Inserts a new PrepopForm into the database
        /// </summary>
        /// <param name="prepopForm">The dummy prepop form to create</param>
        /// <returns>Returns the live PrepopForm after adding to the database.</returns>
        public async Task<PrepopForm> CreatePrepopForm(PrepopForm prepopForm)
        {
            await this.db.InsertAsync(prepopForm);
            return prepopForm;
        }

        /// <summary>
        /// Inserts a new PrepopField into the database
        /// </summary>
        /// <param name="prepopField">The dummy field to create</param>
        /// <returns>Returns the live PrepopField after adding to the database.</returns>
        public async Task<PrepopField> CreatePrepopField(PrepopField prepopField)
        {
            await this.db.InsertAsync(prepopField);
            return prepopField;
        }

        /// <summary>
        /// Gets a list of PrepopForms for a specified User
        /// </summary>
        /// <param name="user">The user for which to retrieve Prepop Forms</param>
        /// <returns>Returns a list of Prepop Forms</returns>
        public async Task<List<PrepopForm>> GetPrepopForms(User user)
        {
            var prepops = await this.db.Table<PrepopForm>().Where(ppf => ppf.User == user.Id).ToListAsync();
            return prepops.Where(x => x.Status == (int)Status.Available).ToList();
        }

        /// <summary>
        /// Gets a list of PrepopFields for the specified PrepopForm
        /// </summary>
        /// <param name="form">The PrepopForm for which to retrieve PrepopFields</param>
        /// <returns>Returns a list of PrepopFields</returns>
        public async Task<List<PrepopField>> GetPrepopFields(PrepopForm form)
        {
            return await this.db.Table<PrepopField>().Where(ppf => ppf.PrepopForm == form.Id).ToListAsync();
        }

        /// <summary>
        /// Gets a list of PrepopForms for a specific form only
        /// </summary>
        /// <param name="form">The Form for which to retrieve Prepop Items</param>
        /// <returns></returns>
        public async Task<List<PrepopForm>> GetPrepopForms(Form form)
        {
            return await this.db.Table<PrepopForm>().Where(ppf => ppf.Form == form.Id).ToListAsync();
        }



        /// <summary>
        /// Deletes a PrepopForm and all PrepopFields associated with it.
        /// </summary>
        /// <param name="prepopForm">The PrepopForm to be deleted.</param>
        /// <returns>Returns a bool to indicate success</returns>
        public async Task<bool> DeletePrepopForm(PrepopForm prepopForm)
        {
            var fields = await this.GetPrepopFields(prepopForm);
            foreach (var field in fields)
            {
                await this.db.DeleteAsync(field);
            }

            await this.db.DeleteAsync(prepopForm);

            return true;
        }

        public async Task<bool> DeletePrepopFormsForApp(Form form)
        {
            List<PrepopForm> prepops = await this.GetPrepopForms(form);
            foreach (var prepop in prepops)
            {
                await this.DeletePrepopForm(prepop);
            }
            return true;
        }

        #endregion

        #region Settings

        /// <summary>
        /// Gets a non-user-specific setting, or creates one with the default value supplied
        /// </summary>
        /// <param name="settingName">The name of the setting to retrieve</param>
        /// <param name="defaultValue">The default value of the setting, if it does not exist already</param>
        /// <returns>Returns the specified setting.</returns>
        public async Task<Setting> GetSettingAsync(string settingName, string defaultValue)
        {
            return await this.GetSettingAsync(null, settingName, defaultValue);
        }

        /// <summary>
        /// Gets a user-specific setting, or creates one with the default value supplied
        /// </summary>
        /// <param name="user">The user for which to get the Setting</param>
        /// <param name="settingName">The name of the setting to retrieve</param>
        /// <param name="defaultValue">The default value of the Setting, if it does not exist already</param>
        /// <returns>Returns the specified setting.</returns>
        public async Task<Setting> GetSettingAsync(User user, string settingName, string defaultValue)
        {
            var userId = user == null ? -1 : user.Id;
            var setting = await this.db.Table<Setting>().Where(s => s.Name == settingName && s.User == userId).FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new Setting
                {
                    Name = settingName,
                    Value = defaultValue,
                    User = user == null ? -1 : user.Id
                };
                await this.db.InsertAsync(setting);
            }
            return setting;
        }

        /// <summary>
        /// Sets a non-user-specific setting to the value provided.
        /// </summary>
        /// <param name="settingName">The setting to save</param>
        /// <param name="value">The value of the setting to save</param>
        /// <returns>Returns the Setting object for the saved setting.</returns>
        public async Task<Setting> SetSettingAsync(string settingName, string value)
        {
            return await this.SetSettingAsync(null, settingName, value);
        }

        /// <summary>
        /// Sets a user-specific setting to the value provided.
        /// </summary>
        /// <param name="user">The user for which to save the setting.</param>
        /// <param name="settingName">The setting to be saved.</param>
        /// <param name="value">The value of the setting to be saved.</param>
        /// <returns>Returns the Setting object for the saved setting.</returns>
        public async Task<Setting> SetSettingAsync(User user, string settingName, string value)
        {
            var userId = user == null ? -1 : user.Id;
            var setting = await this.db.Table<Setting>().Where(s => s.Name == settingName && s.User == userId).FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new Setting
                {
                    Name = settingName,
                    Value = value,
                    User = user == null ? -1 : user.Id
                };
                await this.db.InsertAsync(setting);
                return setting;
            }

            setting.Value = value;
            setting = await this.UpdateItemAsync(setting);
            return setting;
        }

        #endregion
    }
}
