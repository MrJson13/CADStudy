using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcDoNetTools
{
    public static partial class EditTool
    {
        #region 图形编辑封装 改变颜色 复制 旋转
        /// <summary>
        /// 改变图形颜色
        /// </summary>
        /// <param name="c1Id">图形的ObjectId</param>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns>图形的ObjectId</returns> 图形已经添加图形数据库

        public static ObjectId ChangeEntityColor(this ObjectId c1Id, short colorIndex)
        {
            // 图形数据库
            Database db = HostApplicationServices.WorkingDatabase;
            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                // 获取图形对象
                Entity ent1 = (Entity)c1Id.GetObject(OpenMode.ForWrite);
                // 设置颜色
                ent1.ColorIndex = colorIndex;
                trans.Commit();
            }
            return c1Id;
        }
        /// <summary>
        /// 改变图形颜色  图形没有添加到图形数据库
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns></returns>
        public static void ChangeEntityColor(this Entity ent, short colorIndex)
        {
            // 判断图形的IsNewlyObject
            if (ent.IsNewObject)
            {
                ent.ColorIndex = colorIndex;
            }
            // 不是新图形就调用上面的方法
            else
            {
                ent.ObjectId.ChangeEntityColor(colorIndex);
            }
        }
        /// <summary>
        ///  复制图形 图形已经加入到图形数据库中
        /// </summary>
        /// <param name="entId">图形对象的ObjectId</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static Entity CopyEntity(this ObjectId entId, Point3d sourcePoint, Point3d targetPoint)
        {
            // 声明一个图形对象
            Entity entR;
            // 当前图形数据库
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                // Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForWrite);
                // 打开图形
                Entity ent = (Entity)entId.GetObject(OpenMode.ForWrite);
                // 计算变换矩阵
                Vector3d vectoc = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vectoc);
                entR = ent.GetTransformedCopy(mt);
                // 提交事务处理
                trans.Commit();
            }
            return entR;
        }
        /// <summary>
        ///  复制图形 图形没有加到图形数据库中
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="sourcePoint">参考原点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static Entity CopyEntity(this Entity ent, Point3d sourcePoint, Point3d targetPoint)
        {
            //声明一个图形对象
            Entity entR;
            // 判断图形对象的IsNewlyObject属性
            if (ent.IsNewObject)
            {
                // 计算变换矩阵
                Vector3d vector = sourcePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vector);
                entR = ent.GetTransformedCopy(mt);
            }
            else
            {
                entR = ent.ObjectId.CopyEntity(sourcePoint, targetPoint);
            }
            return entR;
        }
        /// <summary>
        /// 旋转图形 图形在数据库中
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="center">旋转中心</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this ObjectId entId, Point3d center, double degree)
        {
            // 当前图形数据库
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                //Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForWrite);
                // 打开图形
                Entity ent = (Entity)entId.GetObject(OpenMode.ForWrite);
                // 计算变换矩阵
                Matrix3d mt = Matrix3d.Rotation(degree.DegreeToAngle(), Vector3d.ZAxis, center);
                ent.TransformBy(mt);
                // 提交事务处理
                trans.Commit();
            }
        }
        /// <summary>
        /// 旋转图形 图形不在数据库中
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="center">旋转中心</param>
        /// <param name="degree">旋转角度</param>
        public static void RotateEntity(this Entity ent, Point3d center, double degree)
        {
            // 判断图形对象的IsNewlyObject属性
            if (ent.IsNewObject)
            {
                // 计算变换矩阵
                Matrix3d mt = Matrix3d.Rotation(degree.DegreeToAngle(), Vector3d.ZAxis, center);
                ent.TransformBy(mt);
            }
            else
            {
                ent.ObjectId.RotateEntity(center, degree);
            }
        }
        #endregion
        #region 图形阵列封装 矩形阵列 环形阵列
        /// <summary>
        /// 矩形阵列 已加到图形数据
        /// </summary>
        /// <param name="entId">图形对象的ObjectId</param>
        /// <param name="rowNum">行数</param>
        /// <param name="columnNum">列数</param>
        /// <param name="disRow">行间距</param>
        /// <param name="disColumn">列间距</param>
        /// <returns>List</returns>  已加入图形数据库
        public static List<Entity> ArrayRectEntity(this ObjectId entId, int rowNum, int columnNum, double disRow, double disColumn)
        {
            // 声明一个Entity类型集合 用于返回
            List<Entity> entList = new List<Entity>();

            // 当前图形数据库
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForWrite);
                // 打开图形
                Entity ent = (Entity)entId.GetObject(OpenMode.ForWrite);

                // 计算变换矩阵
                for (int i = 0; i < rowNum; i++)
                {
                    for (int j = 0; j < columnNum; j++)
                    {
                        Matrix3d mt = Matrix3d.Displacement(new Vector3d(j * disColumn, i * disRow, 0));
                        Entity entA = ent.GetTransformedCopy(mt);
                        btr.AppendEntity(entA);
                        trans.AddNewlyCreatedDBObject(entA, true);
                        entList.Add(entA);
                    }
                }
                ent.Erase(); // 删除多余的图形
                // 提交事务处理
                trans.Commit();
            }
            return entList;
        }
        /// <summary>
        /// 矩形阵列 没有加到图形数据库
        /// </summary>
        /// <param name="entS">图形对象</param>
        /// <param name="rowNum">行数</param>
        /// <param name="columnNum">列数</param>
        /// <param name="disRow">行间距</param>
        /// <param name="disColumn">列间距</param>
        /// <returns>List</returns>  没有加入图形数据库
        public static List<Entity> ArrayRectEntity(this Entity entS, int rowNum, int columnNum, double disRow, double disColumn)
        {
            if (entS.IsNewObject == true)
            {
                // 声明一个Entity类型集合 用于返回
                List<Entity> entList = new List<Entity>();
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // 打开块表
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    // 打开块表记录
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    for (int i = 0; i < rowNum; i++)
                    {
                        for (int j = 0; j < columnNum; j++)
                        {
                            Matrix3d mt = Matrix3d.Displacement(new Vector3d(j * disColumn, i * disRow, 0));
                            Entity entA = entS.GetTransformedCopy(mt);
                            btr.AppendEntity(entA);
                            trans.AddNewlyCreatedDBObject(entA, true);
                            entList.Add(entA);
                        }
                    }
                    trans.Commit();
                }
                return entList;
            }
            else
            {
                return entS.ArrayRectEntity(rowNum, columnNum, disRow, disColumn);
            }

        }
        /// <summary>
        /// 环形阵列 已加到图形数据
        /// </summary>
        /// <param name="entId">图形对象的ObjectId</param>
        /// <param name="num">图形数量</param>
        /// <param name="degree">中心点到各个图形的夹角</param>
        /// <param name="center">中心点</param>
        /// <returns>List</returns>  已经加入图形数据库
        public static List<Entity> ArrayPolarEntity(this ObjectId entId, int num, double degree, Point3d center)
        {
            // 声明一个List集合 用于返回
            List<Entity> entList = new List<Entity>();
            // 打开事务处理
            using (Transaction trans = entId.Database.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(entId.Database.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                Entity ent = (Entity)entId.GetObject(OpenMode.ForWrite);
                // 限定阵列角度大小
                degree = degree > 360 ? 360 : degree;
                degree = degree < -360 ? 360 : degree;
                int divAngnum = num - 1;
                if (degree == 360 || degree == -360)
                {
                    divAngnum = num;
                }
                // 计算变换矩阵
                for (int i = 0; i < num; i++)
                {
                    Matrix3d mt = Matrix3d.Rotation((i * degree / divAngnum).DegreeToAngle(), Vector3d.ZAxis, center);
                    Entity entA = ent.GetTransformedCopy(mt);
                    btr.AppendEntity(entA);
                    trans.AddNewlyCreatedDBObject(entA, true);
                    entList.Add(entA);
                }
                ent.Erase();
                trans.Commit();
            }
            return entList;
        }
        /// <summary>
        /// 环形阵列 没有加到图形数据库
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="num">图形数量</param>
        /// <param name="degree">中心点到各个图形的夹角</param>
        /// <param name="center">中心点</param>
        /// <returns>List</returns>
        public static List<Entity> ArrayPolarEntity(this Entity ent, int num, double degree, Point3d center)
        {
            if (ent.IsNewObject == true)
            {
                // 声明一个List集合 用于返回
                List<Entity> entList = new List<Entity>();
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    // 打开块表
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                    // 打开块表记录
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    degree = degree > 360 ? 360 : degree;
                    degree = degree < -360 ? -360 : degree;
                    int divAngnum = num - 1;
                    if (degree == 360 || degree == -360)
                    {
                        divAngnum = num;
                    }
                    for (int i = 0; i < num; i++)
                    {
                        Matrix3d mt = Matrix3d.Rotation((i * degree / divAngnum).DegreeToAngle(), Vector3d.ZAxis, center);
                        Entity entA = ent.GetTransformedCopy(mt);
                        btr.AppendEntity(entA);
                        trans.AddNewlyCreatedDBObject(entA, true);
                        entList.Add(entA);
                    }
                    trans.Commit();
                }
                return entList;
            }
            else
            {
                return ent.ObjectId.ArrayPolarEntity(num, degree, center);
            }

        }
        #endregion
    }
}
