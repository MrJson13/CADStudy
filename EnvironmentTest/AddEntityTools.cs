using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentTest
{
    public static partial class AddEntityTools
    {
        #region 添加图形封装
        /// <summary>
        /// 添加单个图形文件
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象</param>
        /// <returns></returns>
        public static ObjectId AddEntityToModeSpace(this Database db, Entity ent)
        {
            ObjectId entId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //以读的方式打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //以写打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //加到块表记录
                btr.AppendEntity(ent);
                //更新数据
                trans.AddNewlyCreatedDBObject(ent, true);
                //事务提交
                trans.Commit();
            }
            return entId;
        }
        /// <summary>
        /// 添加多个图形文件
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象</param>
        /// <returns></returns>
        public static ObjectId[] AddEntityToModeSpace(this Database db,params Entity[] ent)
        {
            ObjectId[] entId = new ObjectId[ent.Length];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //以读的方式打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //以写打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                for(int i =0; i < ent.Length; i++)
                {
                    //加到块表记录
                    entId[i] = btr.AppendEntity(ent[i]);
                    //更新数据
                    trans.AddNewlyCreatedDBObject(ent[i], true);
                }               
                //事务提交
                trans.Commit();
            }
            return entId;
        }
        #endregion
        #region 直线绘制封装
        /// <summary>
        /// 根据起始坐标绘制直线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stratPoint">起点坐标</param>
        /// <param name="endPoint">始点坐标</param>
        /// <returns></returns>
        public static ObjectId AddLineToModeSpace(this Database db, Point3d startPoint,Point3d endPoint)
        {
            return db.AddEntityToModeSpace(new Line(startPoint, endPoint));
        }
        /// <summary>
        /// 起点坐标，长度，弧度
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startPoint"></param>
        /// <param name="length"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static ObjectId AddLineToModeSpace(this Database db, Point3d startPoint, Double length,Double degree)
        {
            //计算始点坐标
            double X = startPoint.X + length * Math.Cos(degree.DegreeToAngle());
            double Y = startPoint.Y + length * Math.Sin(degree.DegreeToAngle());
            Point3d endPoint = new Point3d(X, Y, 0);
            return db.AddEntityToModeSpace(new Line(startPoint, endPoint));
        }
        #endregion
        #region 圆弧绘制封装
        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="db"></param>
        /// <param name="center">中心</param>
        /// <param name="radius">半径</param>
        /// <param name="startDegree">起始角度</param>
        /// <param name="endDegree">起始角度</param>
        /// <returns></returns>
        public static ObjectId AddArcToModeSpace(this Database db,Point3d center,double radius,double startDegree,double endDegree)
        {
            return db.AddEntityToModeSpace(new Arc(center, radius, startDegree.DegreeToAngle(), endDegree.DegreeToAngle()));
        }
        /// <summary>
        /// 三点绘制圆弧
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startPoint"></param>
        /// <param name="pointOnArc"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static ObjectId AddArcToModeSpace(this Database db,Point3d startPoint,Point3d pointOnArc,Point3d endPoint)
        {
            //判断是否在同一直线上
            if (startPoint.IsOnOneLine(pointOnArc, endPoint))
            {
                return ObjectId.Null;
            }
            //创建几何对象
            CircularArc3d cArc = new CircularArc3d(startPoint,pointOnArc,endPoint);
            //通过几何对象获取属性
            double radius = cArc.Radius;
            //创建圆弧对象
            Arc arc = new Arc(cArc.Center, cArc.Radius, cArc.Center.GetAngleToXAxis(startPoint), cArc.Center.GetAngleToXAxis(endPoint));
            return db.AddEntityToModeSpace(arc);
        }
        #endregion
        #region 圆的封装
        /// <summary>
        /// 绘制圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static ObjectId AddCircleToModeSpace(this Database db,Point3d center,double radius)
        {
            return db.AddEntityToModeSpace(new Circle((center), new Vector3d(0, 0, 1), radius));
        }
        /// <summary>
        /// 两点画圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static ObjectId AddCircleToModeSpace(this Database db,Point3d point1,Point3d point2)
        {
            //获取两点之间的中心
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);
            //获取半径
            double radius = point1.GetDistanceBetweenTwoPoint(center);
            return db.AddCircleToModeSpace(center, radius);
        }
        /// <summary>
        /// 三点画圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        public static ObjectId AddCircleToModeSpace(this Database db, Point3d point1, Point3d point2, Point3d point3)
        {
            //判断三点是否在同一直线上
            if (point1.IsOnOneLine(point2, point3))
            {
                return ObjectId.Null;
            }
            //声明几何对象Circle3d
            CircularArc3d cArc = new CircularArc3d(point1, point2, point3);
            return db.AddCircleToModeSpace(cArc.Center, cArc.Radius);
        }
        #endregion
        #region 多线段封装
        /// <summary>
        /// 绘制多线段
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isClosed">是否闭合</param>
        /// <param name="contentWidth">线宽</param>
        /// <param name="vertices">多线段的顶点，可变参数</param>
        /// <returns></returns>
        public static ObjectId AddPolyLineToModeSpace(this Database db,bool isClosed,double contentWidth,params Point2d[] vertices)
        {
            //顶点个数小于2无法绘制
            if (vertices.Length < 2)
            {
                return ObjectId.Null;
            }
            //声明一个多线段对象
            Polyline pline = new Polyline();
            //添加多线段顶点
            for (int i = 0; i < vertices.Length; i++)
            {
                pline.AddVertexAt(i, vertices[i], 0, 0, 0);
            }
            if (isClosed)
            {
                pline.Closed = true;
            }
            pline.ConstantWidth = contentWidth;
            return db.AddEntityToModeSpace(pline);
        }
        #endregion
        #region 矩形封装
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1">起点</param>
        /// <param name="point2">对角点</param>
        /// <returns></returns>
        public static ObjectId AddRectToModeSpace(this Database db, Point2d point1, Point2d point2)
        {
            //声明多线段
            Polyline pline = new Polyline();
            //计算矩形的四个顶点坐标
            Point2d p1 = new Point2d(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p2 = new Point2d(Math.Max(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p3 = new Point2d(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
            Point2d p4 = new Point2d(Math.Min(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
            //添加多线段顶点
            pline.AddVertexAt(0, p1, 0, 0,0);
            pline.AddVertexAt(0, p2, 0, 0, 0);
            pline.AddVertexAt(0, p3, 0, 0, 0);
            pline.AddVertexAt(0, p4, 0, 0, 0);
            pline.Closed = true;//闭合
            return db.AddEntityToModeSpace(pline);
        }
        #endregion
        #region 正多边形封装
        /// <summary>
        /// 绘制正多边形
        /// </summary>
        /// <param name="db"></param>
        /// <param name="center">正多边形所在圆圆心</param>
        /// <param name="radius">所在圆半径</param>
        /// <param name="sideNum">变数</param>
        /// <param name="startDegree">起始角度</param>
        /// <returns></returns>
        public static ObjectId AddPolygonToModeSpace(this Database db,Point2d center,double radius,int sideNum,double startDegree)
        {
            //声明一个多线段对象
            Polyline pline = new Polyline();
            //判断变数是否符合
            if (sideNum < 3)
            {
                return ObjectId.Null;
            }
            Point2d[] point = new Point2d[sideNum];
            double angle = startDegree.DegreeToAngle();
            //计算每个顶点坐标
            for (int i = 0; i < sideNum; i++)
            {
                point[i] = new Point2d(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                pline.AddVertexAt(i, point[i], 0, 0, 0);
                angle += Math.PI * 2 / sideNum;
            }
            pline.Closed = true;//闭合
            return db.AddEntityToModeSpace(pline);
        }
        #endregion
        #region 椭圆封装
        /// <summary>
        /// 两点绘制椭圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static ObjectId AddEllipseToModeSpace(this Database db, Point3d point1, Point3d point2)
        {
            //椭圆圆心
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);

            double ratio = Math.Abs((point1.Y - point2.Y) / (point1.X - point2.X));
            Vector3d majorVector = new Vector3d(Math.Abs((point1.X - point2.X)) / 2, 0, 0);
            //声明椭圆对象
            Ellipse elli = new Ellipse(center, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI);
            return db.AddEntityToModeSpace(elli);
        }
        /// <summary>
        /// 三点绘制椭圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="shortRadius"></param>
        /// <returns></returns>
        public static ObjectId AddEllipseToModeSpace(this Database db, Point3d point1, Point3d point2,double shortRadius)
        {
            //椭圆圆心
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);
            //短长轴比例
            double ratio = 2 * shortRadius / point1.GetDistanceBetweenTwoPoint(point2);
            //长轴向量
            Vector3d majorVector = point2.GetVectorTo(center);
            //声明椭圆对象
            Ellipse elli = new Ellipse(center, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI);
            return db.AddEntityToModeSpace(elli);
        }
        #endregion
    }
}
