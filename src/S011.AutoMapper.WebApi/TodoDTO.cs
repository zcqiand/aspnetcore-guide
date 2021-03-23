using System;

namespace AutoMapper.WebApi01
{
    /// <summary>
    /// 待办事项
    /// </summary>
    public class TodoDTO
    {
        #region Public Properties
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        #endregion                
    }
}
