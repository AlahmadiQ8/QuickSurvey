using System;
using System.Collections.Generic;
using QuickSurvey.Core.Entities;

namespace QuickSurvey.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var participants = new List<Participant>
            {
                new Participant
                {
                    Id = 1,
                    UserName = "Momo"
                },
                new Participant
                {
                    Id = 2,
                    UserName = "Mark"
                },
                new Participant
                {
                    Id = 3,
                    UserName = "Scott"
                }
            };
            var choices = new List<Choice>
            {
                new Choice
                {
                    Id = 1,
                    Text = "Choice 1"
                },
                new Choice
                {
                    Id = 2,
                    Text = "Choice 2"
                },
                new Choice
                {
                    Id = 3,
                    Text = "Choice 3"
                }
            };
            var session = new Session
            {
                Id = new Guid(),
                Name = "Test Session",
                Participants = participants,
                Choices = choices
            };


        }
    }
}
