using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    /// <summary>
    /// Test se skládá z mnoha odpovědí, kterým náleží specifické otázky (viz. TestQuestions)
    /// </summary>
    public class TestBigModel
    {
        public List<TestQuestion> TestQuestions { get; set; }   //List TestQuestions (více test questions ne jenom jedna)
    }

    /// <summary>
    /// Každá otázka má více odpovědí, které jí náleží
    /// </summary>
    public class TestQuestion
    {
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
    }

    public class TestResult
    {
        public int CourseId { get; set; }
        public double Percent { get; set; }
    }
}
