import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

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
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png'); // default photo
  currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) {}

// update photo when the user changes the main photo
changeMemberPhoto(photoUrl: string) {
  this.photoUrl.next(photoUrl);
}

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
      const user = response; // the response coming back from the service
      if (user) {
        localStorage.setItem('token', user.token);
        // the foll. is for stringifying the user object that comes
        // from AuthController's Login(). https://www.w3schools.com/js/js_json_stringify.asp
        localStorage.setItem('user', JSON.stringify(user.user));
        // storing the decoded token
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
        this.changeMemberPhoto(this.currentUser.photoUrl);
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
