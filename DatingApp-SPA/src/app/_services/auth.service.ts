import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

// we use service so that the api calls are all centralized and there is no duplication of code in every component's ts file
// this allows us to inject things to the service
@Injectable({
  providedIn: 'root' // root i.e. the app module providing the service
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';

constructor(private http: HttpClient) {}
// model object is the info from the navbar i.e. login box
login(model: any) {

  // for the token coming back from server, we need to use RxJS operators. We need to pass it through the pipe method
  // https://angular.io/guide/rx-library
  // map is for storing token locally for easy access
  // this can be seen in the browser after logging in -> chrome console -> Application -> Local Storage -> token will be present there
  // You can use pipes to link operators together. Pipes let you combine multiple functions into a
  // single function. The pipe() function takes as its arguments the functions you want to combine,
  // and returns a new function that, when executed, runs the composed functions in sequence.
  return this.http.post(this.baseUrl + 'login', model)
  .pipe(
    map((response: any) => {
      const user = response; // the token coming back from the service
      if (user) {
        localStorage.setItem('token', user.token);
      }
    }));
}

register(model: any) {
  return this.http.post(this.baseUrl + 'register', model);
}

}
