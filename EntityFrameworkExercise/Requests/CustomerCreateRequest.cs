﻿using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class CustomerCreateRequest
    {
        public string Name { get; set; } = string.Empty!;
    }
}
