import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from "../book"

interface BookReponse {
  items: Book[];
  totalPages: number;
  totalCount: number;
}

interface PagingConfig {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
}

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  public books: Book[];
  config: PagingConfig;

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private route: ActivatedRoute,
    private router: Router) {

    this.config = {
      currentPage: 1,
      itemsPerPage: 10,
      totalItems: 0
    };

    route.queryParams.subscribe(
      params => {
        this.config.currentPage = params['page'] ? params['page'] : 1;
        this.refresh();
        });

    this.refresh();
  }

  ngOnInit() {
  }

  pageChange(newPage: number) {
    this.router.navigate(['book-list'], { queryParams: { page: newPage } });
  }

  async refresh() {
    let apiPage = this.config.currentPage - 1; // API paging is zero-based
    let response = await this.http.get<BookReponse>(this.baseUrl + 'api/book/page', { params: { page: apiPage.toString(), limit: this.config.itemsPerPage.toString() } }).toPromise();

    this.books = response.items;
    this.config.totalItems = response.totalCount;
  }

  async onDelete(book: Book) {
    await this.http.delete<Book[]>(this.baseUrl + 'api/book/' + book.id).toPromise();
    await this.refresh();
  }
}
