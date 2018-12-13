import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
// the Route Resolver is to prevent loading a null user on ngInit's loadUser (in member detail component)
// and in turn having to use a safe navigation operator ? on every property of user in the html
export class MemberListResolver implements Resolve<User[]> {
    constructor(private userService: UserService, private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        // we don't have to subscribe here because router automatically does it
        return this.userService.getUsers().pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data');
                this.router.navigate(['/home']); // rerouting back to home so that we don't loop infinitely to the members page
                return of(null); // returning observable of null
            })
        );
    }
}