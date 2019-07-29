import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Book } from "../book"

@Component({
  selector: 'app-book-edit',
  templateUrl: './book-edit.component.html',
  styleUrls: ['./book-edit.component.css']
})
export class BookEditComponent implements OnInit {

  model:Book = new Book();

  submitted = false;

  constructor(readonly http: HttpClient, @Inject('BASE_URL') readonly baseUrl: string) {
  }

  ngOnInit() {
  }

  onSubmit() {
    this.http.post<Book>(this.baseUrl + 'api/book', this.model).subscribe(result => {
      this.submitted = true;
    }, error => console.error(error));
  }

  // TODO: Remove this when we're done
  get diagnostic() { return JSON.stringify(this.model); }

}
