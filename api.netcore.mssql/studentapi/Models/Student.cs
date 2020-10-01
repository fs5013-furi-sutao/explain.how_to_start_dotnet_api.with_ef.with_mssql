using System;

namespace studentapi.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}