import { Injectable } from '@angular/core';
import { CanActivate, Router} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
// CanActivate: this will tell our route if it can activate a route we are trying to access
// CanActivate is an interface that a class can implement to be a guard deciding if a route can be activated.
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router,
    private alertify: AlertifyService) {}
  // we can return observable or promise or boolean for canActivate,
  // but since we dont want the other two, we will just have boolean here
  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true; // means we can activate the route
    }

    this.alertify.error('No access. Please login.');
    this.router.navigate(['/home']);
    return false;
  }
}
