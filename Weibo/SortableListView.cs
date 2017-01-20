using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace LogAnalytics
{
    [Serializable]
    public enum SortedListViewFormatType
    {
        String,
        Numeric,
        Date,
        Custom
    }
	internal class RowSorterHelper
	{
		int columnIndex;
		SortedListViewFormatType format;
		
		public RowSorterHelper(int columnIndex, SortedListViewFormatType format)
		{
			this.columnIndex = columnIndex;
			this.format = format;
		}

		public int ColumnIndex
		{
			set { columnIndex = value; }
			get { return columnIndex; }
		}

		public SortedListViewFormatType Format
		{
			set { format = value; }
			get { return format; }
		}
	}


	internal class NumericComparer : IComparer
	{
		SortableListView listView = null;
		
		public NumericComparer(SortableListView lv)
		{
			listView = lv;
		}

		public int Compare(object obj1, object obj2)
		{
			ListViewItem item1 = (ListViewItem)obj1;
			ListViewItem item2 = (ListViewItem)obj2;
			string string1 = item1.Text;
			string string2 = item2.Text;											
			if ( listView.LastSortedColumn != 0 )
			{
				if (item1.SubItems.Count > listView.LastSortedColumn)
					string1 = item1.SubItems[listView.LastSortedColumn].Text;
				else
					string1 = "";

				if (item2.SubItems.Count>listView.LastSortedColumn)
					string2 = item2.SubItems[listView.LastSortedColumn].Text;
				else
					string2 = "";
			}

			if (string1 == "") string1 = "0";
			if (string2 == "") string2 = "0";

			Decimal dec1 = decimal.Parse(string1);
			Decimal dec2 = decimal.Parse(string2);
			int result = dec1.CompareTo(dec2);
			if ( listView.SortingOrder == SortOrder.Descending)
				result *= -1;
			return result;
		}
	}

	internal class StringComparer : IComparer
	{
		SortableListView listView = null;

		public StringComparer(SortableListView lv)
		{
			listView = lv;
		}

		public int Compare(object obj1, object obj2)
		{
			ListViewItem item1 = (ListViewItem)obj1;
			ListViewItem item2 = (ListViewItem)obj2;
			string string1 = item1.Text;
			string string2 = item2.Text;											
			if ( listView.LastSortedColumn != 0 )
			{
				if (item1.SubItems.Count > listView.LastSortedColumn)
					string1 = item1.SubItems[listView.LastSortedColumn].Text;
				else
					string1 = "";

				if (item2.SubItems.Count>listView.LastSortedColumn)
					string2 = item2.SubItems[listView.LastSortedColumn].Text;
				else
					string2 = "";
			}
			int result = string1.CompareTo(string2);
			if ( listView.SortingOrder == SortOrder.Descending)
				result *= -1;
			return result;
		}
	}

	internal class DateTimeComparer : IComparer
	{
		SortableListView listView = null;

		public DateTimeComparer(SortableListView lv)
		{
			listView = lv;
		}

		public int Compare(object obj1, object obj2)
		{
			ListViewItem item1 = (ListViewItem)obj1;
			ListViewItem item2 = (ListViewItem)obj2;
			string string1 = item1.Text;
			string string2 = item2.Text;											
			if ( listView.LastSortedColumn != 0 )
			{
				if (item1.SubItems.Count > listView.LastSortedColumn)
					string1 = item1.SubItems[listView.LastSortedColumn].Text;
				else
					string1 = "";

				if (item2.SubItems.Count>listView.LastSortedColumn)
					string2 = item2.SubItems[listView.LastSortedColumn].Text;
				else
					string2 = "";
			}
			if (string1.Length == 0 && string2.Length == 0)
				return 0;
			if (string1.Length == 0)
				return -1;
			if (string2.Length == 0)
				return 1;
			DateTime date1 = DateTime.Parse(string1);
			DateTime date2 = DateTime.Parse(string2);
			int result = DateTime.Compare(date1, date2);
			if ( listView.SortingOrder == SortOrder.Descending)
				result *= -1;
			return result;
		}
	}
	public delegate void PLMCustomCompare(object obj);
//	public class CustomComparer : IComparer
//	{
//		SortableListView listView = null;
//		public PLMCustomCompare CustomCompare;
//		public CustomComparer(SortableListView lv)
//		{
//			listView = lv;
//		}
//		public int Compare(object obj1, object obj2)
//		{
//			if(CustomCompare!=null)
//				return CustomCompare(obj1,obj2);
//			else
//				return 0;
//		}
//	}

//	public class CompareListItems : IComparer
//	{
//		SortableListView listView = null;
//		
//		public CompareListItems(SortableListView lv)
//		{
//			listView = lv;
//		}
//
//		public int Compare(object obj1, object obj2)
//		{
//			ListViewItem item1 = (ListViewItem)obj1;
//			ListViewItem item2 = (ListViewItem)obj2;
//			RowSorterHelper rs = listView.GetRowSorterHelper();
//			string string1 = item1.Text;
//			string string2 = item2.Text;
//			int result = 0;
//												
//			if ( listView.LastSortedColumn != 0 )
//			{
//				if (item1.SubItems.Count>listView.LastSortedColumn)
//					string1 = item1.SubItems[listView.LastSortedColumn].Text;
//				else
//					string1 = "";
//				if (item2.SubItems.Count>listView.LastSortedColumn)
//					string2 = item2.SubItems[listView.LastSortedColumn].Text;
//				else
//					string2 = "";
//			}
//
//			if ( rs != null )
//			{
//				if ( rs.Format == SortedListViewFormatType.String)
//					result = CompareStrings(string1, string2, listView.Sorting);
//				else if ( rs.Format == SortedListViewFormatType.Numeric )
//					result = CompareNumbers(string1, string2, listView.Sorting);
//				else if ( rs.Format == SortedListViewFormatType.Date )
//					result = CompareDates(string1, string2, listView.Sorting);
//			}
//			else
//			{
//				result = CompareStrings(string1, string2, listView.Sorting);
//			}
//			return result;
//		}
//
//
//		int CompareStrings(string string1, string string2, SortOrder sortOrder)
//		{
//			int result = string.Compare(string1, string2);
//			if ( sortOrder == SortOrder.Descending)
//				result *= -1;
//			return result;
//		}
//
//		int CompareNumbers(string string1, string string2, SortOrder sortOrder)
//		{
//			Decimal dec1 = decimal.Parse(string1);
//			decimal dec2 = decimal.Parse(string2);
//			int result = dec1.CompareTo(dec2);
//			if ( sortOrder == SortOrder.Descending)
//				result *= -1;
//			return result;
//			
//		}
//
//		int CompareDates(string string1, string string2, SortOrder sortOrder)
//		{
//			DateTime date1 = DateTime.Parse(string1);
//			DateTime date2 = DateTime.Parse(string2);
//			int result = DateTime.Compare(date1, date2);
//			if ( sortOrder == SortOrder.Descending)
//				result *= -1;
//			return result;
//			
//		}
//	}

	/// <summary>
	/// 可排序的ListView。
	/// 2004.5.8 添加对ColumnHeader鼠标右击事件
	/// </summary>
	public class SortableListView : ListView
	{
		private const int WM_NOTIFY =  0x004E;
		private const int WM_COMMAND = 0x0111;
		private const int WM_USER = 0x0400;
		private const int NM_FIRST = 0;
		private const int NM_RCLICK  = NM_FIRST - 5;
		private const int WM_NCRBUTTONDOWN = 0x00A4;

		private SortOrder m_sortingorder = SortOrder.None;
		public SortOrder SortingOrder 
		{
			get { return this.m_sortingorder; }
			set { this.m_sortingorder = value; }
		}

		public class ColumnHeaderRClickEventArgs 
		{
		
			private int x;
			private int y;

			public int X
			{
				get { return this.x;}
				set { this.x = value;}
			}
			public int Y
			{
				get { return this.y;}
				set { this.y = value;}
			}

			public ColumnHeaderRClickEventArgs(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		public delegate void ColumnHeaderRClickEventHandler(object sender, ColumnHeaderRClickEventArgs e);

		public ColumnHeaderRClickEventHandler ColumnHeaderRClick;

		protected virtual void OnColumnHeaderRClick(ColumnHeaderRClickEventArgs e)
		{
			if (ColumnHeaderRClick != null)
				ColumnHeaderRClick(this, e);
		}

        //protected override void WndProc(ref System.Windows.Forms.Message m)
        //{
        //    base.WndProc (ref m);

        //    if (m.Msg == WM_NOTIFY) //WM_NOTIFY
        //    {
        //        Win32.NMHDR hdr = (Win32.NMHDR)Marshal.PtrToStructure(
        //            m.LParam,typeof(Win32.NMHDR));
        //        if (hdr.code == NM_RCLICK) //NM_RCLICK
        //        {
        //            Point pt = this.PointToClient(Cursor.Position);
        //            this.OnColumnHeaderRClick(new ColumnHeaderRClickEventArgs(pt.X, pt.Y)); 
        //        }
        //    }
        //}


		protected ArrayList rowSorterList = new ArrayList();
		public int LastSortedColumn = 0;
		private bool sortable = true;
		public PLMCustomCompare CustomCompare;

		public SortableListView():base()
		{
			this.HideSelection = false;
			this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnColumnClick);
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles(); 
		}

		private void OnColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			if (!sortable)
				return;

			LastSortedColumn = e.Column;
			RowSorterHelper rs = this.GetRowSorterHelper();
			if ( rs == null)
				this.ListViewItemSorter = new StringComparer(this);
			else if ( rs.Format == SortedListViewFormatType.String)
				this.ListViewItemSorter = new StringComparer(this);
			else if ( rs.Format == SortedListViewFormatType.Numeric )
				this.ListViewItemSorter = new NumericComparer(this);
			else if ( rs.Format == SortedListViewFormatType.Date )
				this.ListViewItemSorter = new DateTimeComparer(this);
            else if ( rs.Format == SortedListViewFormatType.Custom)
				if(this.CustomCompare!=null)
				{
					this.CustomCompare(this);
					return;
				}
			this.Sort();

			this.ListViewItemSorter = null;

			if (this.SortingOrder == SortOrder.Descending)
				this.SortingOrder = SortOrder.Ascending;
			else
				this.SortingOrder = SortOrder.Descending;


		}

		protected int CompareFromSubClass = 0;
		internal RowSorterHelper GetRowSorterHelper()
		{
			if (CompareFromSubClass == 1)
			{
				SortedListViewFormatType format = this.GetSortedFormatType(this.LastSortedColumn);
				return new RowSorterHelper(this.LastSortedColumn, format);
			}
			for ( int i = 0; i < rowSorterList.Count; i++ )
			{
				RowSorterHelper rs = (RowSorterHelper)rowSorterList[i];
				if ( rs != null && rs.ColumnIndex == LastSortedColumn )
				{
					return rs;
				}
			}
			return null;
		}


		public virtual SortedListViewFormatType GetSortedFormatType(int columnIndex)
		{
			return SortedListViewFormatType.String;
		}

		public void SetColumnSortFormat(int columnIndex, SortedListViewFormatType format)
		{
			rowSorterList.Add(new RowSorterHelper(columnIndex, format));
		}		

		/// <summary>
		/// 设置是否需要排序。
		/// </summary>
		public bool Sortable
		{
			set	{	sortable = value;	}
		}
	}
}
