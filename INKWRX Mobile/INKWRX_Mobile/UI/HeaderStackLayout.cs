using FormTools.FormDescriptor;
using INKWRX_Mobile.Util;
using INKWRXPhotoTools_Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace INKWRX_Mobile.UI
{
    public class HeaderStackLayout : StackLayout
    {
        public HeaderStackLayout(HeaderPanelDescriptor panel, FormRenderer renderer) : base()
        {
            
            this.Descriptor = panel;
            this.FormRenderer = renderer;
            var headerLabel = new TextLabelView(panel.HeaderLabel);

            this.HeaderFrame = new Frame
            {
                HasShadow = false,
                OutlineColor = panel.HeaderStroke.ToColor(),
                BackgroundColor = panel.HeaderBackground.ToColor(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = headerLabel
            };
            
            this.Padding = 0;
            this.Spacing = 0;
            this.Children.Add(this.HeaderFrame);
            this.ChildFrame = new Frame
            {
                BackgroundColor = ElementDescriptor.WhiteColour.ToColor(),
                OutlineColor = panel.HeaderStroke.ToColor(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HasShadow = false
            };
            if (panel.Children.Count == 0)
            {
                var child = new Label();
                child.Text = " ";
                this.ChildFrame.Content = child;
            }
            else
            {
                var innerPanel = new StackLayout();
                innerPanel.VerticalOptions = LayoutOptions.CenterAndExpand;
                innerPanel.HorizontalOptions = LayoutOptions.FillAndExpand;
                

                var labels = new List<TextLabelView>();
                this.FieldList = new List<IElementView>();
                var subPanels = new List<HeaderStackLayout>();
                var allViews = new List<View>();

                var layoutElements = new List<IElementView>();

                List<RectangleView> rectElements = new List<RectangleView>();
                Dictionary<RectangleView, StackLayout> rectStacks = new Dictionary<RectangleView, StackLayout>();
                Dictionary<RectangleView, List<View>> rectViews = new Dictionary<RectangleView, List<View>>();
                foreach (var child in panel.Children)
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

                        if (shouldContinueLayout)
                        {
                            if (child is ImageDescriptor)
                            {
                                layoutElements.Add(new FormImageView((ImageDescriptor)child, renderer.FormDescriptor.FormId.ToString(), renderer.User.Username));
                                continue;
                            }
                            else
                            {
                                var thisRect0 = new RectangleView((RectangleDescriptor)child);
                                rectElements.Add(thisRect0);
                                rectViews.Add(thisRect0, new List<View>());
                                rectStacks.Add(thisRect0, new StackLayout
                                {
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                    HorizontalOptions = LayoutOptions.StartAndExpand
                                });
                                thisRect0.Content = rectStacks[thisRect0];
                                layoutElements.Add(thisRect0);
                                continue;
                            }
                        }
                        else
                        {
                            var grid = new ElementLayoutGrid(layoutElements);
                            allViews.Add(grid);
                            layoutElements = new List<IElementView>();

                            if (child is ImageDescriptor)
                            {
                                layoutElements.Add(new FormImageView((ImageDescriptor)child, renderer.FormDescriptor.FormId.ToString(), renderer.User.Username));
                                continue;
                            }
                            else if (child is RectangleDescriptor)
                            {
                                var thisRect1 = new RectangleView((RectangleDescriptor)child);
                                rectElements.Add(thisRect1);
                                rectViews.Add(thisRect1, new List<View>());
                                rectStacks.Add(thisRect1, new StackLayout
                                {
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                    HorizontalOptions = LayoutOptions.StartAndExpand
                                });
                                thisRect1.Content = rectStacks[thisRect1];
                                layoutElements.Add(thisRect1);
                                continue;
                            }
                        }
                    }
                    else if (child is ImageDescriptor)
                    {
                        layoutElements.Add(new FormImageView((ImageDescriptor)child, renderer.FormDescriptor.FormId.ToString(), renderer.User.Username));
                        continue;
                    }
                    else if (child is RectangleDescriptor)
                    {
                        var thisRect2 = new RectangleView((RectangleDescriptor)child);
                        rectElements.Add(thisRect2);
                        rectViews.Add(thisRect2, new List<View>());
                        rectStacks.Add(thisRect2, new StackLayout
                        {
                            VerticalOptions = LayoutOptions.StartAndExpand,
                            HorizontalOptions = LayoutOptions.StartAndExpand
                        });
                        thisRect2.Content = rectStacks[thisRect2];
                        layoutElements.Add(thisRect2);
                        continue;
                    }

                    if (child is string)
                    {
                        continue;
                    }
                    if (child is List<RadioButtonDescriptor>)
                    {
                        List<RadioButtonDescriptor> radioGroupDescriptorList = (List<RadioButtonDescriptor>)child;
                        List<RadioButtonFieldView> radioGroupViewList = new List<RadioButtonFieldView>();
                        var mandatory = false;
                        var topY = radioGroupDescriptorList.Select(x => x.Origin.Y).Min();
                        var bottomY = radioGroupDescriptorList.Select(x => x.Origin.Y + x.Height).Max();
                        var addToRect1 = false;
                        RectangleView thisRect3 = null;
                        foreach (var rect in rectElements)
                        {
                            if (topY > rect.Descriptor.Origin.Y && topY < (rect.Descriptor.Origin.Y + rect.Descriptor.Height))
                            {
                                thisRect3 = rect;
                                addToRect1 = true;
                                break;
                            }
                        }
                        foreach (RadioButtonDescriptor RadioButtonDescriptor in radioGroupDescriptorList)
                        {
                            RadioButtonFieldView radioButtonFieldView = new RadioButtonFieldView(RadioButtonDescriptor);
                            radioGroupViewList.Add(radioButtonFieldView);
                            if (mandatory || RadioButtonDescriptor.Mandatory)
                            {
                                mandatory = true;
                                radioButtonFieldView.Mandatory = true;
                            }
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
                            if (addToRect1)
                            {
                                rectViews[thisRect3].Add(radioButtonFieldView);
                            }
                            else
                            {
                                allViews.Add(radioButtonFieldView);
                            }
                            FieldList.Add(radioButtonFieldView);
                            renderer.AddField(radioButtonFieldView);
                        }
                        continue;
                    }
                    var v = this.FormRenderer.ProcessChild(child);
                    if (v == null) continue; // TODO: Remove this when complete
                    RectangleView thisRect = null;
                    var addToRect = false;
                    foreach (var rect in rectElements)
                    {
                        if ((v is TextLabelView
                            && ((TextLabelView)v).Descriptor.Origin.Y > rect.Descriptor.Origin.Y
                            && ((TextLabelView)v).Descriptor.Origin.Y < (rect.Descriptor.Origin.Y + rect.Descriptor.Height))
                            || (v is IElementView
                                && ((IElementView)v).RawDescriptor.Origin.Y > rect.Descriptor.Origin.Y
                                && ((IElementView)v).RawDescriptor.Origin.Y < (rect.Descriptor.Origin.Y + rect.Descriptor.Height))
                            ) {
                            thisRect = rect;
                            addToRect = true;
                            break;
                        }
                    }

                    
                    if (addToRect)
                    {
                        rectViews[thisRect].Add(v);
                    }
                    else
                    {
                        allViews.Add(v);
                    }
                    if (v is TextLabelView)
                    {
                        labels.Add((TextLabelView)v);
                    }
                    else if (v is HeaderStackLayout)
                    {
                        subPanels.Add((HeaderStackLayout)v);
                        this.FormRenderer.AddPanel((HeaderStackLayout)v);
                    }
                    else
                    {
                        if (v is IElementView)
                        {
                            FieldList.Add((IElementView)v);
                            renderer.AddField((IElementView)v);
                        }
                    }
                }

                if (rectElements.Any())
                {
                    AddElementsToShape(rectElements, rectViews, rectStacks);
                }

                if (layoutElements.Count > 0)
                {
                    var grid = new ElementLayoutGrid(layoutElements);
                    allViews.Add(grid);
                    layoutElements = new List<IElementView>();
                }

                allViews = allViews.OrderBy(x => x is IElementView ? ((IElementView)x).RawDescriptor.Origin.Y : x is TextLabelView ? ((TextLabelView)x).Descriptor.Origin.Y : x.Y).ToList();
                
                foreach (var v in allViews)
                {
                    innerPanel.Children.Add(v);
                }

                this.ChildFrame.Content = innerPanel;

            }
            this.Children.Add(this.ChildFrame);

            this.PanelShown = false;
        }

        private static void AddElementsToShape(List<RectangleView> rects, Dictionary<RectangleView, List<View>> lastRectViews, Dictionary<RectangleView, StackLayout> lastRectStack)
        {

            foreach (var rect in rects)
            {
                var views = lastRectViews[rect].OrderBy(x => x is IElementView ? ((IElementView)x).RawDescriptor.Origin.Y : x is TextLabelView ? ((TextLabelView)x).Descriptor.Origin.Y : x.Y).ToList();
                foreach (var view in views)
                {
                    lastRectStack[rect].Children.Add(view);
                }
            }
        }

        public Frame ChildFrame { get; private set; }

        public Frame HeaderFrame { get; private set; }
        

        private FormRenderer FormRenderer = null;

        public bool PanelShown
        {
            get { return Children[1].IsVisible; }
            set { Children[1].IsVisible = value; }
        }

        public HeaderPanelDescriptor Descriptor { get; set; }

        public List<IElementView> FieldList { get; set; }
    }
}
