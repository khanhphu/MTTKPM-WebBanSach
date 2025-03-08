using Project8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project8
{
    // Interface chung cho cả sách vật lý và sách điện tử
    interface LibrarySystem
    {
        function findBook(id)
        function borrowBook(id, studentId, borrowDate)
        function returnBook(id, returnDate)
        function checkStatus(id)
    }

    // Lớp đại diện cho sách vật lý
    class Book implements LibrarySystem
    {
        function findBook(id) {
            // Tìm sách theo ID trong hệ thống
        }

        function borrowBook(id, studentId, borrowDate) {
            // Đánh dấu sách là đã mượn
        }

        function returnBook(id, returnDate) {
            // Cập nhật trạng thái sách là đã trả
        }

        function checkStatus(id) {
            // Kiểm tra sách có sẵn hay không
        }
    }

    // Lớp của bên thứ ba quản lý sách điện tử
    class EBook
    {
        function searchById(id)
        {
            // Tìm kiếm sách điện tử theo ID
        }

        function downloadBook(id, userId)
        {
            // Cấp quyền tải sách điện tử
        }

        function checkAvailability(id)
        {
            // Kiểm tra xem sách điện tử có sẵn hay không
        }

        function releaseAccess(userId, id)
        {
            // Thu hồi quyền truy cập sách điện tử
        }

        function logUsage(id, duration)
        {
            // Ghi lại thời gian sử dụng sách điện tử
        }
    }

    // Adapter để tích hợp EBook vào LibrarySystem
    class EBookAdapter implements LibrarySystem
    {
    private ebook: EBook

    function findBook(id)
    {
        return ebook.searchById(id)
    }

    function borrowBook(id, studentId, borrowDate)
    {
        return ebook.downloadBook(id, studentId)
    }

    function returnBook(id, returnDate)
    {
        return ebook.releaseAccess(studentId, id)
    }

    function checkStatus(id)
    {
        return ebook.checkAvailability(id)
    }
}

// Quản lý thư viện, chứa cả sách vật lý và sách điện tử
class LibraryManager
{
    private books: List<Book>
    private ebooks: List<EBookAdapter>

    void addBook(book: Book)
    {
        books.add(book)
    }

    void addEBook(ebook: EBookAdapter)
    {
        ebooks.add(ebook)
    }

    void borrowItem(id, studentId, borrowDate)
    {
        if (findBookInLibrary(id))
        {
            findBookInLibrary(id).borrowBook(id, studentId, borrowDate)
        }
    }

    void returnItem(id, returnDate)
    {
        if (findBookInLibrary(id))
        {
            findBookInLibrary(id).returnBook(id, returnDate)
        }
    }

    void checkItemStatus(id)
    {
        if (findBookInLibrary(id))
        {
            return findBookInLibrary(id).checkStatus(id)
        }
    }

    void function findBookInLibrary(id)
    {
        foreach book in books {
            if book.findBook(id) != null then return book
        }
        foreach ebook in ebooks {
            if ebook.findBook(id) != null then return ebook
        }
        return null
    }
}

}

// Interface chung cho cả sách vật lý và sách điện tử
interface LibrarySystem
{
    void findBook(id)
    void  borrowBook(id, studentId, borrowDate)
    void  returnBook(id, returnDate)
    void  checkStatus(id)
}

// Lớp đại diện cho sách vật lý
class Book implements LibrarySystem
{
    void findBook(id) {
        // Tìm sách theo ID trong hệ thống
    }

    void borrowBook(id, studentId, borrowDate) {
        // Đánh dấu sách là đã mượn
    }

    void returnBook(id, returnDate) {
        // Cập nhật trạng thái sách là đã trả
    }

    void checkStatus(id) {
        // Kiểm tra sách có sẵn hay không
    }
}
// Lớp của bên thứ ba quản lý sách điện tử
class EBook
{
    void searchById(id)
    {
        // Tìm kiếm sách điện tử theo ID
    }

    void downloadBook(id, userId)
    {
        // Cấp quyền tải sách điện tử
    }

    void checkAvailability(id)
    {
        // Kiểm tra xem sách điện tử có sẵn hay không
    }

    void releaseAccess(userId, id)
    {
        // Thu hồi quyền truy cập sách điện tử
    }

    void logUsage(id, duration)
    {
        // Ghi lại thời gian sử dụng sách điện tử
    }
}



// Adapter để tích hợp EBook vào LibrarySystem
class EBookAdapter implements LibrarySystem
{
    private ebook: EBook

    void findBook(id) {
        return ebook.searchById(id)
    }

    void borrowBook(id, studentId, borrowDate)
{
    return ebook.downloadBook(id, studentId)
    }

void returnBook(id, returnDate)
{
    return ebook.releaseAccess(studentId, id)
    }

void checkStatus(id)
{
    return ebook.checkAvailability(id)
    }
}