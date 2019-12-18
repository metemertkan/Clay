using System;
using Clay.Models.Domain.Base;

namespace Clay.Models.Domain
{
    [Serializable]
    public class Attempt : BaseModel
    {
        public long Id { get; set; }
        public Guid LockId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public bool IsSuccessful { get; set; }
    }
}