using API.Domain.Entities;
using API.Domain.Interfaces;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly BookManagementContext _context;
        public BookService(BookManagementContext context)
        {
            _context = context;
        }

        public void AddBook(Book book)
        {
            _context.books.Add(book);
            _context.SaveChanges();
        }

        public Book? GetBookById(int id)
        {
            return _context.books.Where(v => v.Id == id).FirstOrDefault();
        }

        public List<Book> GetAllBooks(int page = 1, string bookName = null, string authorName = null)
        {
            var query = _context.books.AsQueryable();
            if(!string.IsNullOrEmpty(bookName))
            {
                query = query.Where(v => v.BookName.ToLower().Contains(bookName));
            }

            int pageItems = 10;
            query = query.Skip((page - 1) * pageItems).Take(pageItems);

            return query.ToList();
        }

        public void UpdateBook(Book book)
        {
            _context.books.Update(book);
            _context.SaveChanges();
        }

        public void DeleteBook(Book book)
        {
            _context.books.Remove(book);
            _context.SaveChanges();
        }  
    }
}
