﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Multitouch.Framework.WPF.Controls
{
	/// <summary>
	/// Places child elements at random position.
	/// </summary>
	public class RandomCanvas : Canvas
	{
		bool isArranging;

		/// <summary>
		/// Identifies <see cref="RandomizePositionsProperty"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty RandomizePositionsProperty = DependencyProperty.Register("RandomizePositions", typeof(bool),
			typeof(RandomCanvas), new UIPropertyMetadata(true));

		static RandomCanvas()
		{
			LeftProperty.OverrideMetadata(typeof(RandomCanvas), new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged));
			TopProperty.OverrideMetadata(typeof(RandomCanvas), new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged));
			RightProperty.OverrideMetadata(typeof(RandomCanvas), new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged));
			BottomProperty.OverrideMetadata(typeof(RandomCanvas), new FrameworkPropertyMetadata(double.NaN, OnPositioningChanged));
		}

		/// <summary>
		/// Gets or sets a value indicating whether items position will be randomized.
		/// </summary>
		/// <value><c>true</c> if items position will be randomized; otherwise, <c>false</c>.</value>
		public bool RandomizePositions
		{
			get { return (bool)GetValue(RandomizePositionsProperty); }
			set { SetValue(RandomizePositionsProperty, value); }
		}

		private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement reference = d as UIElement;
			if (reference != null)
			{
				RandomCanvas parent = VisualTreeHelper.GetParent(reference) as RandomCanvas;
				if (parent != null && !parent.isArranging)
					parent.InvalidateArrange();
			}
		}

		/// <summary>
		/// Arranges the content of a <see cref="T:System.Windows.Controls.Canvas"/> element.
		/// </summary>
		/// <param name="arrangeSize">The size that this <see cref="T:System.Windows.Controls.Canvas"/> element should use to arrange its child elements.</param>
		/// <returns>
		/// A <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.Canvas"/> element and its descendants.
		/// </returns>
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			isArranging = true;

			foreach (UIElement child in InternalChildren)
			{
				if (child == null)
					continue;

				double x = 0;
				double y = 0;

				// if left is set - use left
				double left = GetLeft(child);
				if (!double.IsNaN(left))
					x = left;
				else
				{
					// if right is set - use right
					double right = GetRight(child);
					if (!double.IsNaN(right))
						x = arrangeSize.Width - child.DesiredSize.Width - right;
					else if (RandomizePositions)
						x = Utility.NextRandom() * (arrangeSize.Width - (child.DesiredSize.Width / 2)); // if left and right not set - use random.
				}

				// if top is set - use top
				double top = GetTop(child);
				if (!double.IsNaN(top))
					y = top;
				else
				{
					// if bottom is set - use bottom
					double bottom = GetBottom(child);
					if (!double.IsNaN(bottom))
						y = arrangeSize.Height - child.DesiredSize.Height - bottom;
					else if (RandomizePositions)
						y = Utility.NextRandom() * (arrangeSize.Height - (child.DesiredSize.Height / 2)); // if top and right not set - use random
				}

				child.Arrange(new Rect(new Point(x, y), child.DesiredSize));

				SetLeft(child, x);
				SetTop(child, y);
			}

			isArranging = false;

			return arrangeSize;
		}
	}
}