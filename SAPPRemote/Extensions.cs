// <copyright file="Extensions.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Extensions class.</summary>
namespace SAPPRemote
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Collections;

	public static class IEnumerableExtensions
	{
		public static bool ContentsAreIdentical(this Players<SAPPRemote.PlayerData> items)
		{
			object lastItem = null;
			foreach (object item in items) {
				if (lastItem != null && !lastItem.Equals(item))
					return false;
				lastItem = item;
			}
			return true;
		}
	}

	public static class ListBoxExtension
	{
		public static void AddItem(this ListBox lb, string text, int index, bool waitUntilReturn = false)
		{
			var lbit = new ListBoxItem();
			lbit.Content = text;
			lbit.Tag = index;
			Action additem = () => lb.Items.Add(lbit);
			if (lb.CheckAccess()) {
				additem();
			} else if (waitUntilReturn) {
				lb.Dispatcher.Invoke(additem);
			} else {
				lb.Dispatcher.BeginInvoke(additem);
			}
		}
		
		public static void RemoveItem(this ListBox lb, string text, int index, bool waitUntilReturn = false)
		{
			//ListBoxItem lbit;
			//lbit.Content = text;
			//lbit.Tag = index;
			Action additem = () => lb.Items.Remove("");
			if (lb.CheckAccess()) {
				additem();
			} else if (waitUntilReturn) {
				lb.Dispatcher.Invoke(additem);
			} else {
				lb.Dispatcher.BeginInvoke(additem);
			}
		}
	}
    
	public static class WindowExtensions
	{
		public static void SetTitle(this Window winx, string text, bool waitUntilReturn = false)
		{
			Action settitle = () => winx.Title = "SAPP Remote" + text;
			if (winx.CheckAccess()) {
				settitle();
			} else if (waitUntilReturn) {
				winx.Dispatcher.Invoke(settitle);
			} else {
				winx.Dispatcher.BeginInvoke(settitle);
			}
		}
    	
	}

	public static class TextBoxExtensions
	{
		public static void CheckAppendText(this TextBoxBase textBox, string msg, bool waitUntilReturn = false)
		{
 
			Action append = () => textBox.AppendText(msg);
			if (textBox.CheckAccess()) {
				append();
			} else if (waitUntilReturn) {
				textBox.Dispatcher.Invoke(append);
			} else {
				textBox.Dispatcher.BeginInvoke(append);
			}
		}
	}

	public static class TextBlockExtensions
	{
		public static void SetText(this TextBlock textBlock, string text, bool waitUntilReturn = false)
		{

			Action append = () => textBlock.Text = text;
			if (textBlock.CheckAccess()) {
				append();
			} else if (waitUntilReturn) {
				textBlock.Dispatcher.Invoke(append);
			} else {
				textBlock.Dispatcher.BeginInvoke(append);
			}
		}
	}
}
