using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public enum Fetch
    {
        /// <summary>
        /// 默认值,以实体的属性特性中配置的为准
        /// </summary>
        Default=0,
        /// <summary>
        /// 一律不加载引用和集合属性
        /// </summary>
        NONE=1,
        /// <summary>
        /// 加载对象引用,不加载集合
        /// </summary>
        REFS=2,
        /// <summary>
        /// 加载集合不加载对象引用
        /// </summary>
        SETS=3,
        /// <summary>
        /// 加载对象引用和集合
        /// </summary>
        REFSandSets=4
    }
}
