import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

// the following is to send the http/API request with the token so that the API calls
// can be successful. The Authorization/Bearer part we have already seen in Postman
// this was replaced by angular jwt token part (section 9 lecture 86) with the jwt config
// code in app.module.ts
// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl; // getting from environment.ts

constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
  // get returns type of Observable of objects. So we need to cast it to User[]
  // the foll. was commented due to adding jwt token part in app.module.ts
  // return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
  return this.http.get<User[]>(this.baseUrl + 'users');
}

getUser(id): Observable<User> {
  // the foll. was commented due to adding jwt token part in app.module.ts
  // return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}
}
