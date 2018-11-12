import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}; // object for username and password, used in the template i.e. html

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    // subscribe should be used for things that return an observable
    this.authService.login(this.model).subscribe(next => {
      console.log('Logged in successfully');
    }, error => {
      console.log('Failed to login');
    });
  }
  
  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; // !! will return true or false i.e. shorthand for an if statement
  }

  // when user logs out we need to delete token from local storage
  logout() {
    localStorage.removeItem('token');
    console.log('Logged out');
  }

}
