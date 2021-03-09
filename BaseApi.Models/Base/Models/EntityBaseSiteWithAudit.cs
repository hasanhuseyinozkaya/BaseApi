using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Models
{
    public class EntityBaseSiteWithAudit<T> : EntityBase<T>
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public Guid CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
