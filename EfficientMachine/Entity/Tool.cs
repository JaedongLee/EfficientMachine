﻿namespace EfficientMachine.Entity
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RuntimeEnvironment { get; set; }
        public string ReleaseType { get; set; }
        public string MainProgramLocation { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}