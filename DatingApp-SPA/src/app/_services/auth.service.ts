import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';

// we use service so that the api calls are all centralized and there is no duplication of code in every component's ts file
// this allows us to inject things to the service
@Injectable({
  providedIn: 'root' // root i.e. the app module providing the service.
  // Providing service at the root or AppModule level means it is registered with the root injector.
  // There is just one service instance in the entire app and every class that injects service
  // gets this service instance (refer to the constructors of nav component and register component having the authService instance)
  // unless you configure another provider with a child injector.
  // link: https://angular.io/guide/dependency-injection
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

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
        // storing the decoded token
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        console.log(this.decodedToken);
      }
    }));
}

register(model: any) {
  return this.http.post(this.baseUrl + 'register', model);
}

// moving this from its initial placement of nav component, since we might be reusing this in other pages in future
// so we dont want to keep unnecessarily injecting nav component in the other places - sec 6 lecture 54
loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

}
