using System;

namespace Clay.Models.Domain
{
    public class Attempt : IModel
    {
        public long Id { get; set; }
        public Guid LockId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
        public bool IsSuccessful { get; set; }
    }
}