using System;
using Clay.Data.Pagination;

namespace Clay.Models.InputModels.Admin
{
    public class GetLockHistoryModel
    {
        public PagedModel PagedModel { get; set; }
        public Guid LockId { get; set; }
    }
}