
using System;
using System.Collections.Generic;

class Student
{
    public string Name { get; set; }
    public List<double> Scores { get; set; }

    public Student(string name, List<double> scores)
    {
        Name = name;
        Scores = scores;
    }

    public double CalculateAverage()
    {
        double sum = 0;

        for (int i = 0; i < Scores.Count; i++)
        {
            sum += Scores[i];
        }

        return sum / Scores.Count;
    }

    public string GetGrade()
    {
        double average = CalculateAverage();

        if (average >= 90)
            return "A";
        else if (average >= 80)
            return "B";
        else if (average >= 70)
            return "C";
        else if (average >= 60)
            return "D";
        else
            return "F";
    }

    public void PrintReport()
    {
        Console.WriteLine("===== Student Report Card =====");
        Console.WriteLine("Student Name: " + Name);
        Console.WriteLine("-------------------------------");

        for (int i = 0; i < Scores.Count; i++)
        {
            Console.WriteLine("Subject " + (i + 1) + ": " + Scores[i]);
        }

        Console.WriteLine("-------------------------------");
        Console.WriteLine("Average: " + CalculateAverage().ToString("F2"));
        Console.WriteLine("Grade: " + GetGrade());
        Console.WriteLine("===============================");
    }
}

class Program
{
    static void Main()
    {
        List<double> scores = new List<double>
        {
            85, 90, 78, 95, 82
        };

        Student student = new Student("Ali", scores);

        student.PrintReport();
    }
}
