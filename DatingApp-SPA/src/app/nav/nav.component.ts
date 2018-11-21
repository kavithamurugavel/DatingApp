import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}; // object for username and password, used in the template i.e. html

  // making authService public so that we don't get errors accessing it from nav.component.html
  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    // subscribe should be used for things that return an observable
    // subscribe is like listening to an event
    // A set of RxJS operators i.e. pipe, map, etc applied to an observable is a recipe—that is,
    // a set of instructions for producing the values you’re interested in.
    // By itself, the recipe doesn’t do anything. You need to call subscribe() to produce a result through the recipe.
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged in successfully');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }
  // first version of loggedIn method
  // loggedIn() {
  //   const token = localStorage.getItem('token');
  //   return !!token; // !! will return true or false i.e. shorthand for an if statement
  // }

  // when user logs out we need to delete token from local storage
  logout() {
    localStorage.removeItem('token');
    this.alertify.message('Logged out');
  }

}
