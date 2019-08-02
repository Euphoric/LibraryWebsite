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
      this.http.get<Book>(this.baseUrl + 'api/book/' + this.editId).subscribe(result => {
        this.model = result
      }, error => console.error(error));
    }
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
