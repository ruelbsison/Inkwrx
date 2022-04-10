using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace INKWRX_Mobile.iOS.CustomRenderers
{
	public class FormListItemRenderer : ViewCellRenderer
	{
		public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);

			cell.BackgroundView.BackgroundColor = Color.Silver.ToUIColor();

			return cell;
		}
	}
}
