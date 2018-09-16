using System;

namespace LogicBuilder.Domain
{
    abstract public class BaseModel
    {
        public EntityStateType EntityState { get; set; }
    }
}
