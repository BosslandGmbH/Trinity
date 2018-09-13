using System;
namespace Trinity.UI.UIComponents
{
    public class DropTargetAdorners
  {
    public static Type Highlight => typeof(DropTargetHighlightAdorner);

      public static Type Insert => typeof(DropTargetInsertionAdorner);
  }
}