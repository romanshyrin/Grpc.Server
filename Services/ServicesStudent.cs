using Grpc.Core;
using GrpcServer.Data;
using GrpcServer.Model;
using GrpcServer.Protos;

namespace GrpcServer.Services;

public class ServicesStudent : RemoteStudent.RemoteStudentBase
{
    private readonly ILogger<ServicesStudent> _logger;
    private readonly DataContext _context;

    public ServicesStudent(ILogger<ServicesStudent> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }
    

    public override Task<StudentModel> GetStudentInfo(StudentLookupModel request, ServerCallContext context)
    {
        StudentModel output = new StudentModel();

        var student = _context.Students.Find(request.StudentId);

        _logger.LogInformation("Отправка ответа");

        if (student != null)
        {
            output.StudentId = student.StudentId;
            output.FirstName = student.FirstName;
            output.LastName = student.LastName;
            output.Email = student.Email;
        }

        return Task.FromResult(output);
    }
    public override Task<Reply> InsertStudent(StudentModel request, ServerCallContext context)
    {
        var s = _context.Students.Find(request.StudentId);

        if (s != null)
        {
            return Task.FromResult(
              new Reply()
              {
                  Result = $"Студент {request.FirstName} {request.LastName} уже существует.",
                  IsOk = false
              }
           );
        }

        Student student = new Student()
        {
            StudentId = request.StudentId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
        };

        _logger.LogInformation("Вставить студента");

        try
        {
            _context.Students.Add(student);
            var returnVal = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }

        return Task.FromResult(
           new Reply()
           {
               Result = $"Студент {request.FirstName} {request.LastName}  был успешно добавлен.",
               IsOk = true
           }
        );
    }

    public override Task<Reply> UpdateStudent(StudentModel request, ServerCallContext context)
    {
        var s = _context.Students.Find(request.StudentId);

        if (s == null)
        {
            return Task.FromResult(
             new Reply()
             {
                 Result = $"Студент {request.FirstName} {request.LastName} Не найден.",
                 IsOk = false
             }
            );
        }

        s.FirstName = request.FirstName;
        s.LastName = request.LastName;
        s.Email = request.Email;

        _logger.LogInformation("Обновить студента");

        try
        {
            var returnVal = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }

        return Task.FromResult(
           new Reply()
           {
               Result = $"Студент {request.FirstName} {request.LastName} был успешно обновлен.",
               IsOk = true
           }
        );
    }

    public override Task<Reply> DeleteStudent(StudentLookupModel request, ServerCallContext context)
    {
        var s = _context.Students.Find(request.StudentId);

        if (s == null)
        {
            return Task.FromResult(
              new Reply()
              {
                  Result = $"Студент с ID {request.StudentId} Не найден.",
                  IsOk = false
              }
           );
        }

        _logger.LogInformation("Удалить студента");

        try
        {
            _context.Students.Remove(s);
            var returnVal = _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }

        return Task.FromResult(
           new Reply()
           {
               Result = $"Студент с ID {request.StudentId} был успешно удален.",
               IsOk = true
           }
        );
    }

    public override Task<StudentList> RetrieveAllStudents(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("Получить всех студентов");

        StudentList list = new StudentList();

        try
        {
            List<StudentModel> studentList = new List<StudentModel>();

            var students = _context.Students.ToList();

            foreach (var c in students)
            {
                studentList.Add(new StudentModel()
                {
                    StudentId = c.StudentId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                });
            }

            list.Items.AddRange(studentList);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.ToString());
        }

        return Task.FromResult(list);
    }
}