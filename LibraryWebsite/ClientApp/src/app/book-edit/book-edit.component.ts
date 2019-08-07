import { Component, OnInit, Inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Book } from "../book"

@Component({
  selector: 'app-book-edit',
  templateUrl: './book-edit.component.html',
  styleUrls: ['./book-edit.component.css']
})
export class BookEditComponent implements OnInit {

  editId:string
  model:Book = new Book();

  submitted = false;

  get isEdit() {
    return this.editId != null;
  }

  constructor(readonly http: HttpClient, @Inject('BASE_URL') readonly baseUrl: string, route: ActivatedRoute) {
    this.editId = route.snapshot.paramMap.get('id');

    if (this.isEdit) {
      this.loadBook();
    }
  }

  ngOnInit() {
  }

  async loadBook() {
    this.model = await this.http.get<Book>(this.baseUrl + 'api/book/' + this.editId).toPromise();
  }

  async onSubmit() {
    if (!this.isEdit) {
      await this.http.post<Book>(this.baseUrl + 'api/book', this.model).toPromise();
    } else {
      await this.http.put<Book>(this.baseUrl + 'api/book/' + this.editId, this.model).toPromise();
    }

    this.submitted = true;
  }

  // TODO: Remove this when we're done
  get diagnostic() { return JSON.stringify(this.model); }

}
