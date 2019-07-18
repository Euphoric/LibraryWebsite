import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  public books: Book[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Book[]>(baseUrl + 'api/book').subscribe(result => {
      this.books = result;
    }, error => console.error(error));
  }

  ngOnInit() {
  }

}

interface Book {
  id: string;
  title: string;
  author: string;
  description: string;
  iban: string;
}
