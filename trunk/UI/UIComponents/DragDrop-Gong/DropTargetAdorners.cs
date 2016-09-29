using System;

namespace Trinity.UI.UIComponents
{
  public class DropTargetAdorners
  {
    public static Type Highlight
    {
      get { return typeof(DropTargetHighlightAdorner); }
    }

    public static Type Insert
    {
      get { return typeof(DropTargetInsertionAdorner); }
    }
  }
}