using FormTools.FormDescriptor;
using INKWRX_Mobile.Database.Entity;
using INKWRX_Mobile.UI;
using INKWRX_Mobile.Views;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.Util
{
    public class FormRenderer
    {
        public FormRenderer(FormDescriptor descriptor, StackLayout formContent, User user)
        {
            this.FormDescriptor = descriptor;
            this.FormContent = formContent;
            this.FormRendered = false;
            this.User = user;
        }

        public event FieldAddedEventHandler FieldAdded;
        public event PanelAddedEventHandler PanelAdded;
        public event FormRenderCompleteHandler FormRenderComplete;

        
        public void RenderForm()
        {
            var allElements = new Dictionary<object, double>();
            foreach (var child in this.FormDescriptor.PageDescriptors[0].Panels.OfType<HeaderPanelDescriptor>())
            {
                allElements.Add(child, child.RectArea.Y);
            }
            foreach (var child in this.FormDescriptor.PageDescriptors[0].FieldDescriptors)
            {
                allElements.Add(child, child.Origin.Y);
            }
            foreach (var child in this.FormDescriptor.PageDescriptors[0].ShapeDescriptors)
            {
                allElements.Add(child, child.Origin.Y);
            }
            foreach (var child in this.FormDescriptor.PageDescriptors[0].ImageDescriptors)
            {
                allElements.Add(child, child.Origin.Y);
            }
            foreach (var child in this.FormDescriptor.PageDescriptors[0].TextLabelDescriptors)
            {
                allElements.Add(child, child.Origin.Y);
            }

            var layoutElements = new List<IElementView>();

            // iterate over all descriptors again, but this time in Y order of each element
            foreach (var child in allElements.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList())
            {
                if (layoutElements.Count > 0)
                {
                    bool shouldContinueLayout = true;
                    if (child is ImageDescriptor || child is RectangleDescriptor)
                    {
                        // another image
                        var topY = layoutElements.Select(x => x.RawDescriptor.Origin.Y).Min();
                        var bottomY = layoutElements.Select(x => x.RawDescriptor.Origin.Y + x.RawDescriptor.Height).Max();
                        if (child is ImageDescriptor)
                        {
                            if (((ImageDescriptor)child).Origin.Y > bottomY)
                            {
                                shouldContinueLayout = false;
                            }
                        }
                        else
                        {
                            if (((RectangleDescriptor)child).Origin.Y > bottomY)
                            {
                                shouldContinueLayout = false;
                            }
                        }
                        
                    }
                    else
                    {
                        shouldContinueLayout = false;
                    }

                    if (!shouldContinueLayout)
                    {
                        var grid = new ElementLayoutGrid(layoutElements);
                        this.FormContent.Children.Add(grid);
                        layoutElements = new List<IElementView>();
                    }  
                }
                if (child is ImageDescriptor)
                {
                    layoutElements.Add(new FormImageView((ImageDescriptor)child, this.FormDescriptor.FormId.ToString(), this.User.Username));
                    continue;
                }
                else if (child is RectangleDescriptor)
                {
                    layoutElements.Add(new RectangleView((RectangleDescriptor)child));
                    continue;
                }


                if (child is HeaderPanelDescriptor)
                {
                    var headerPanel = new HeaderStackLayout((HeaderPanelDescriptor)child, this);
                    this.FormContent.Children.Add(headerPanel);
                    this.AddPanel(headerPanel);
                }
                else
                {
                    if (child is List<RadioButtonDescriptor>)
                    {
                        List<RadioButtonDescriptor> radioGroupDescriptorList = (List<RadioButtonDescriptor>)child;
                        List<RadioButtonFieldView> radioGroupViewList = new List<RadioButtonFieldView>();
                        foreach (RadioButtonDescriptor RadioButtonDescriptor in radioGroupDescriptorList)
                        {
                            RadioButtonFieldView radioButtonFieldView = new RadioButtonFieldView(RadioButtonDescriptor);
                            radioGroupViewList.Add(radioButtonFieldView);

                            TapGestureRecognizer tapRadioRecognizer = new TapGestureRecognizer();
                            tapRadioRecognizer.Tapped += (sender, args) =>
                            {
                                radioButtonFieldView.IsOn = true;

                                foreach (RadioButtonFieldView radioGroupField in radioGroupViewList)
                                {
                                    if (radioGroupField.RawDescriptor.FieldId != RadioButtonDescriptor.FieldId)
                                    {
                                        radioGroupField.IsOn = false;
                                    }
                                }
                            };
                            radioButtonFieldView.GestureRecognizers.Add(tapRadioRecognizer);
                            this.FormContent.Children.Add(radioButtonFieldView);
                            this.AddField(radioButtonFieldView);
                        }
                        continue;
                    }
                    var v = ProcessChild(child);
                    if (v == null) continue;



                    this.FormContent.Children.Add(v);
                    this.AddField((IElementView)v);
                    
                    
                }
            }

            if (layoutElements.Count > 0)
            {
                var grid = new ElementLayoutGrid(layoutElements);
                this.FormContent.Children.Add(grid);
                layoutElements = new List<IElementView>();
            }

            this.FormRenderComplete?.Invoke(this, new EventArgs());
        }

        public View ProcessChild(object child)
        {
            if (child is HeaderPanelDescriptor)
            {
                var v = new HeaderStackLayout((HeaderPanelDescriptor)child, this);
                return v;
            }
            if (child is TextLabelDescriptor)
            {
                var v = new TextLabelView((TextLabelDescriptor)child);
                return v;
            }
            if (child is DateTimeFieldDescriptor)
            {
                //has to be above "if (child is ISOFieldDescriptor)"
                DateTimeFieldView dateTimeFieldView = new DateTimeFieldView((DateTimeFieldDescriptor)child);
                return dateTimeFieldView;
            }
            if (child is DecimalFieldDescriptor)
            {
                var decimalFieldView = new DecimalFieldView((DecimalFieldDescriptor)child);
                if (((ISOFieldDescriptor)child).IsCalcField)
                {
                    decimalFieldView.IsEnabled = false;
                    decimalFieldView.BackgroundColor = CoreAppTools.CalculationGreen;
                    foreach (CalcList calcList in this.FormDescriptor.PageDescriptors[0].PageCalcFields)
                    {
                        if (calcList.FieldName.Equals(((ISOFieldDescriptor)child).FdtFieldName))
                        {
                            calcList.FieldView = decimalFieldView;
                            if (!calcList.Descriptor.Calc.Contains("#"))
                            {
                                //no input calc
                                Calculations.GetInstance().PredifinedCalc(calcList);
                            }
                            break;
                        }
                    }
                }
                return decimalFieldView;
            }

            if (child is ISOFieldDescriptor)
            {
                var v = new ISOFieldView((ISOFieldDescriptor)child);
                if (((ISOFieldDescriptor)child).IsCalcField)
                {
                    v.IsEnabled = false;
                    v.BackgroundColor = CoreAppTools.CalculationGreen;
                    v.TextColor = Color.Black;
                    foreach (CalcList calcList in this.FormDescriptor.PageDescriptors[0].PageCalcFields)
                    {
                        if (calcList.FieldName.Equals(((ISOFieldDescriptor)child).FdtFieldName))
                        {
                            calcList.FieldView = v;
                            if (!calcList.Descriptor.Calc.Contains("#"))
                            {
                                //no input calc
                                Calculations.GetInstance().PredifinedCalc(calcList);
                            }
                            break;
                        }
                    }
                }
                return v;
            }
            if (child is DrawingFieldDescriptor)
            {
                var v = new DrawingFieldView((DrawingFieldDescriptor)child);
                return v;
            }
            if (child is TabletImageDescriptor)
            {
                var v = new TabletImageView((TabletImageDescriptor)child, ((FormViewPage)App.Current.MainPage).Processor);
                return v;
            }
            if (child is TickBoxDescriptor)
            {
                return new TickBoxFieldView((TickBoxDescriptor)child);
            }
            if (child is DropdownDescriptor)
            {
                DropDownFieldView dropDownView = new DropDownFieldView((DropdownDescriptor)child);
                dropDownView.ProcessLexicon(this.FormDescriptor.FormId, ((App)App.Current).LoggedInUser.Username);
                return dropDownView;
            }
            if (child is RectangleDescriptor || child is RoundedRectangleDescriptor)
            {
                return new RectangleView((RectangleDescriptor)child);
            }
            if (child is NotesFieldDescriptor)
            {
                return new NotesFieldView((NotesFieldDescriptor)child);
            }
            if (child is ImageDescriptor)
            {
                return new FormImageView((ImageDescriptor)child, string.Format("{0}", this.FormDescriptor.FormId), this.User.Username);
            }
            return null;
        }


        public void AddField(IElementView field)
        {
            this.FieldAdded?.Invoke(new FieldAddedEventArgs { Element = field });
            field.FieldValueChanged += FieldValueChanged;
        }

        private async void FieldValueChanged(IElementView sender, EventArgs args)
        {
            this.DataChanged = true;

			await Task.Run(() =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
                    Calculations.GetInstance().Recalculate(
						new List<string> { sender.RawDescriptor.FdtFieldName },
						this.FormDescriptor.PageDescriptors[0].PageCalcFields,
						this.FormContent.Children.OfType<HeaderStackLayout>());
				});
			}).ConfigureAwait(true);
            
        }

        public void AddPanel(HeaderStackLayout headerPanel)
        {
            this.PanelAdded?.Invoke(new PanelAddedEventArgs { HeaderPanel = headerPanel });
        }

        public bool FormRendered { get; set; }
        public StackLayout FormContent { get; set; }
        public FormDescriptor FormDescriptor { get; set; }
        
        public bool DataChanged { get; set; }
        public User User { get; private set; }
    }

    public delegate void FieldAddedEventHandler(FieldAddedEventArgs eventArgs);
    public delegate void PanelAddedEventHandler(PanelAddedEventArgs eventArgs);
    public delegate void FormRenderCompleteHandler(object sender, EventArgs eventArgs);

    public class FieldAddedEventArgs : EventArgs
    {
        public IElementView Element { get; set; }
    }

    public class PanelAddedEventArgs : EventArgs
    {
        public HeaderStackLayout HeaderPanel { get; set; }
    }
}
