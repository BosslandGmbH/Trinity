using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace _3dsToXaml
{
    class ModelBase : UIElement3D
    {
        public ModelBase(string resourceKey)
        {
            this.Visual3DModel = Application.Current.Resources[resourceKey] as Model3DGroup;
            Debug.Assert(this.Visual3DModel != null);
        }

        public void Move(double offsetX, double offsetY, double offsetZ, double angle)
        {
            Transform3DGroup transform = new Transform3DGroup();
            RotateTransform3D rotateTrans = new RotateTransform3D();
            rotateTrans.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle);
            TranslateTransform3D translateTrans = new TranslateTransform3D(offsetX, offsetY, offsetZ);
            transform.Children.Add(rotateTrans);
            transform.Children.Add(translateTrans);
            this.Transform = transform;
        }
    }
}
