using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicBuilder.Data
{
    abstract public class BaseData
    {
        [NotMapped]
        public EntityStateType EntityState { get; set; }
    }
}
