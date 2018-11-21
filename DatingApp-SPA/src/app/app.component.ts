import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import {JwtHelperService} from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  // we are injecting authService here so that when the application loads, we will
  // get the token from the local storage and set the token inside auth service when our application first loads
  // this is to correctly display the name in Welcome *User*
  constructor(private authService: AuthService) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      // decoding the token from the local storage on initial load
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
  }
}
