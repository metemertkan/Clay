using System;
using System.ComponentModel.DataAnnotations;
using Clay.Data.Pagination;

namespace Clay.Models.InputModels.Admin
{
    public class GetLockHistoryModel
    {
        public PagedModel PagedModel { get; set; }
        [Required]
        public Guid LockId { get; set; }
    }
}