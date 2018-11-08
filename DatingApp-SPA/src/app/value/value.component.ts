import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  values: any;

  // declaring an http var used below
  constructor(private http: HttpClient) { }

  // loading during initialization
  ngOnInit() {
    this.getValues();
  }

  // the api call from the Asp.net API
  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(Response => {
      this.values = Response;
    }, error => {
      console.log(error);
    });
  }

}
