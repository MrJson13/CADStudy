using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AcDoNetTools;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;

namespace DemoCAD
{
    public class demo
    {
        #region Day6
        /// <summary>
        /// dll测试
        /// </summary>
        [CommandMethod("TestColor")]
        public void TestColor()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            //调用图形绘制
            db.AddCircleModeSpace(new Point3d(100, 100, 0), 50);

            Circle c1 = new Circle(new Point3d(100, 100, 0), new Vector3d(0, 0, 1), 50);
            Circle c2 = new Circle(new Point3d(200, 100, 0), new Vector3d(0, 0, 1), 50);

            c1.ColorIndex = 1;
            c2.Color = Color.FromRgb(23, 156, 255);

            db.AddEntityToModeSpace(c1, c2);
        }
        #endregion
    }
}
