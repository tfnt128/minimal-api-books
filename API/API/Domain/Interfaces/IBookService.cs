using API.Domain.DTOs;
using API.Domain.Entities;

namespace API.Domain.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks(int page = 1, string? bookName = null, string? authorName = null);
        Book? GetBookById(int id);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
    }
}
