import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

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

getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
  // since PaginatedResult is a class, we have to create a new instance here
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

  let params = new HttpParams();

  // this check is technically not reqd. since our API sends pageNumber = 1 and itemsPerPage = 10 by default
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  // for filtering section 14 lecture 144
  if (userParams != null) {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }

  // get returns type of Observable of objects. So we need to cast it to User[]
  // the foll. was commented due to adding jwt token part in app.module.ts
  // return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);

  // observe: response will give us access to HttpResponse. We are including the response headers & HttpParam in get() to
  // get the users as well as the pagination information from the response headers and store them in our PaginatedResult class.
  return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
  .pipe (
    map(response => {
      paginatedResult.result = response.body; // these will be the users
      if (response.headers.get('Pagination') != null) { // if the Pagination header from the API is not null
        // converting serialized string format of the headers into Json object here
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}

getUser(id): Observable<User> {
  // the foll. was commented due to adding jwt token part in app.module.ts
  // return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User) {
  // PUT: update the user on the server. Returns the updated user upon success.
  return this.http.put(this.baseUrl + 'users/' + id, user);
}

// post requires a body so we are just giving an empty object {} to satisfy that
setMainPhoto(userID: number, id: number) {
  return this.http.post(this.baseUrl + 'users/' + userID + '/photos/' + id + '/setMain', {});
}

deletePhoto(userID: number, id: number) {
  return this.http.delete(this.baseUrl + 'users/' + userID + '/photos/' + id);
}
}
