using System.Globalization;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class GradingServiceTests
{
    private readonly GradingService gradingService = new();

    // Helpers

    private static TestQuestion MakeQuestion(string? correctAnswer, float score = 10f) =>
        new() { QuestionAnswer = correctAnswer, QuestionScore = score };

    private static Answer MakeAnswer(string value) => new() { Value = value };

    // GradeSingleChoice

    [Fact]
    public void GradeSingleChoice_CorrectAnswer_SetsCorrectValue()
    {
        string correctAnswere = "B";
        var question = MakeQuestion(correctAnswere, score: 5f);
        var answer = MakeAnswer(correctAnswere);

        gradingService.GradeSingleChoice(question, answer);

        Assert.Equal("CORRECT:5", answer.Value);
    }

    [Fact]
    public void GradeSingleChoice_CorrectAnswerWithWhitespace_TrimsAndMarksCorrect()
    {
        var question = MakeQuestion("  B  ", score: 5f);
        var answer = MakeAnswer(" B ");

        gradingService.GradeSingleChoice(question, answer);

        Assert.Equal("CORRECT:5", answer.Value);
    }

    [Fact]
    public void GradeSingleChoice_WrongAnswer_LeavesValueUnchanged()
    {
        string correctAnswer = "A", wrongAnswer = "B";
        var question = MakeQuestion(wrongAnswer);
        var answer = MakeAnswer(correctAnswer);

        gradingService.GradeSingleChoice(question, answer);

        Assert.Equal(correctAnswer, answer.Value);
    }

    [Fact]
    public void GradeSingleChoice_NullQuestionAnswer_LeavesValueUnchanged()
    {
        string correctAnswer = "A";
        var question = MakeQuestion(null);
        var answer = MakeAnswer(correctAnswer);

        gradingService.GradeSingleChoice(question, answer);

        Assert.Equal(correctAnswer, answer.Value);
    }

    // GradeText

    [Fact]
    public void GradeText_ExactMatch_SetsCorrectValue()
    {
        string correctAnswer = "Paris";
        var question = MakeQuestion(correctAnswer, score: 10f);
        var answer = MakeAnswer(correctAnswer);

        gradingService.GradeText(question, answer);

        Assert.Equal("CORRECT:10", answer.Value);
    }

    [Fact]
    public void GradeText_CaseInsensitiveMatch_SetsCorrectValue()
    {
        var question = MakeQuestion("paris");
        var answer = MakeAnswer("PARIS");

        gradingService.GradeText(question, answer);

        Assert.StartsWith("CORRECT:", answer.Value);
    }

    [Fact]
    public void GradeText_WhitespaceTrimmed_SetsCorrectValue()
    {
        var question = MakeQuestion("  Paris  ");
        var answer = MakeAnswer(" paris ");

        gradingService.GradeText(question, answer);

        Assert.StartsWith("CORRECT:", answer.Value);
    }

    [Fact]
    public void GradeText_WrongAnswer_LeavesValueUnchanged()
    {
        var question = MakeQuestion("Paris");
        var answer = MakeAnswer("London");

        gradingService.GradeText(question, answer);

        Assert.Equal("London", answer.Value);
    }

    [Fact]
    public void GradeText_NullQuestionAnswer_LeavesValueUnchanged()
    {
        var question = MakeQuestion(null);
        var answer = MakeAnswer("Paris");

        gradingService.GradeText(question, answer);

        Assert.Equal("Paris", answer.Value);
    }

    // GradeTrueFalse

    [Fact]
    public void GradeTrueFalse_CorrectAnswer_SetsCorrectValue()
    {
        var question = MakeQuestion("true", score: 4f);
        var answer = MakeAnswer("true");

        gradingService.GradeTrueFalse(question, answer);

        Assert.Equal("CORRECT:4", answer.Value);
    }

    [Fact]
    public void GradeTrueFalse_CaseInsensitiveMatch_SetsCorrectValue()
    {
        var question = MakeQuestion("true");
        var answer = MakeAnswer("TRUE");

        gradingService.GradeTrueFalse(question, answer);

        Assert.StartsWith("CORRECT:", answer.Value);
    }

    [Fact]
    public void GradeTrueFalse_WrongAnswer_LeavesValueUnchanged()
    {
        var question = MakeQuestion("true");
        var answer = MakeAnswer("false");

        gradingService.GradeTrueFalse(question, answer);

        Assert.Equal("false", answer.Value);
    }

    [Fact]
    public void GradeTrueFalse_NullQuestionAnswer_LeavesValueUnchanged()
    {
        var question = MakeQuestion(null);
        var answer = MakeAnswer("true");

        gradingService.GradeTrueFalse(question, answer);

        Assert.Equal("true", answer.Value);
    }

    // GradeMultipleChoice

    [Fact]
    public void GradeMultipleChoice_AllCorrectSelected_AwardsFullScore()
    {
        // correct: [0, 1] — selecting both correct answers, no wrong ones
        var question = MakeQuestion("[0, 1]", score: 10f);
        var answer = MakeAnswer("[0, 1]");

        gradingService.GradeMultipleChoice(question, answer);

        Assert.Equal("PARTIAL:10", answer.Value);
    }

    [Fact]
    public void GradeMultipleChoice_OneCorrectOneWrongSelected_PartialScore()
    {
        // correct: [0, 1], wrong options: [2, 3] (assuming OptionsPerQuestion = 4)
        // scorePerCorrect = 10 / 2 = 5
        // penaltyPerWrong = 10 / 2 = 5
        // selected [0, 2]: +5 for 0, -5 for 2 = 0, clamped to 0
        var question = MakeQuestion("[0, 1]", score: 10f);
        var answer = MakeAnswer("[0, 2]");

        gradingService.GradeMultipleChoice(question, answer);

        var value = answer.Value;
        Assert.StartsWith("PARTIAL:", value);
        var score = float.Parse(value["PARTIAL:".Length..], CultureInfo.InvariantCulture);
        Assert.True(score >= 0f);
    }

    [Fact]
    public void GradeMultipleChoice_NoCorrectSelected_ScoreClampedToZero()
    {
        var question = MakeQuestion("[0, 1]", score: 10f);
        var answer = MakeAnswer("[2, 3]");

        gradingService.GradeMultipleChoice(question, answer);

        Assert.Equal("PARTIAL:0", answer.Value);
    }

    [Fact]
    public void GradeMultipleChoice_NothingSelected_ScoreIsZero()
    {
        var question = MakeQuestion("[0, 1]", score: 10f);
        var answer = MakeAnswer("[]");

        gradingService.GradeMultipleChoice(question, answer);

        Assert.Equal("PARTIAL:0", answer.Value);
    }

    [Fact]
    public void GradeMultipleChoice_NullQuestionAnswer_LeavesValueUnchanged()
    {
        var question = MakeQuestion(null);
        var answer = MakeAnswer("[0, 1]");

        gradingService.GradeMultipleChoice(question, answer);

        Assert.Equal("[0, 1]", answer.Value);
    }

    [Fact]
    public void GradeMultipleChoice_AlwaysSetsPartialPrefix()
    {
        var question = MakeQuestion("[0]", score: 10f);
        var answer = MakeAnswer("[0]");

        gradingService.GradeMultipleChoice(question, answer);

        Assert.StartsWith("PARTIAL:", answer.Value);
    }

    [Fact]
    public void GradeMultipleChoice_NegativeScoreClampedToZero()
    {
        // selecting all wrong options should never produce a negative result
        var question = MakeQuestion("[0]", score: 10f);
        var answer = MakeAnswer("[1, 2, 3]");

        gradingService.GradeMultipleChoice(question, answer);

        var score = float.Parse(answer.Value["PARTIAL:".Length..], CultureInfo.InvariantCulture);
        Assert.True(score >= 0f);
    }

    // CalculateFinalScore

    [Fact]
    public void CalculateFinalScore_OnlyCorrectAnswers_SumsScores()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer>
            {
                MakeAnswer("CORRECT:10"),
                MakeAnswer("CORRECT:5"),
            }
        };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(15f, result);
        Assert.Equal(15m, attempt.Score);
    }

    [Fact]
    public void CalculateFinalScore_OnlyPartialAnswers_SumsScores()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer>
            {
                MakeAnswer("PARTIAL:3.5"),
                MakeAnswer("PARTIAL:6.5"),
            }
        };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(10f, result, precision: 4);
        Assert.Equal(10m, attempt.Score);
    }

    [Fact]
    public void CalculateFinalScore_MixedCorrectPartialAndUngraded_SumsOnlyScoredAnswers()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer>
            {
                MakeAnswer("CORRECT:10"),
                MakeAnswer("PARTIAL:4"),
                MakeAnswer("London"),   // wrong answer, not graded yet
            }
        };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(14f, result, precision: 4);
    }

    [Fact]
    public void CalculateFinalScore_NoGradedAnswers_ReturnsZero()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer>
            {
                MakeAnswer("London"),
                MakeAnswer("A"),
            }
        };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(0f, result);
        Assert.Equal(0m, attempt.Score);
    }

    [Fact]
    public void CalculateFinalScore_EmptyAnswerList_ReturnsZero()
    {
        var attempt = new TestAttempt { Answers = new List<Answer>() };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(0f, result);
    }

    [Fact]
    public void CalculateFinalScore_CorrectPrefixCaseInsensitive_CountsScore()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer> { MakeAnswer("correct:8") }
        };

        var result = gradingService.CalculateFinalScore(attempt);

        Assert.Equal(8f, result);
    }

    [Fact]
    public void CalculateFinalScore_SetsAttemptScoreAsDecimal()
    {
        var attempt = new TestAttempt
        {
            Answers = new List<Answer> { MakeAnswer("CORRECT:7.5") }
        };

        gradingService.CalculateFinalScore(attempt);

        Assert.Equal(7.5m, attempt.Score);
    }
}
