import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Book } from "../book"

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  public books: Book[];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.refresh();
  }

  ngOnInit() {
  }

  async refresh() {
    this.books = await this.http.get<Book[]>(this.baseUrl + 'api/book').toPromise();
  }

  async onDelete(book: Book) {
    await this.http.delete<Book[]>(this.baseUrl + 'api/book/' + book.id).toPromise();
    await this.refresh();
  }
}
