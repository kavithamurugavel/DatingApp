import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

// array of Routes and each route is an object
// ordering of the routes is important, for eg: we shouldn't have the wildcard route line first
// because it would just look at the wildcard route and ignore the other routes
export const appRoutes: Routes = [
    // instead of giving path: 'home' we give an empty string so that home means localhost:4200 and not localhost:4200/home
    {path: '', component: HomeComponent},
    // creating a dummy route and include child routes for that, so that we can
    // use guard the routes using canActivate: [AuthGuard] in one place rather than replicate that line many times
    {
        path: '', // so that the url will be localhost:5000/members instead of localhost:5000/*somestring*members
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard], // adding our can activate route guard here
        children: [
            {path: 'members', component: MemberListComponent},
            {path: 'messages', component: MessagesComponent},
            {path: 'lists', component: ListsComponent}
        ]
    },
    // the following will be a wildcard route
    // pathMatch full means we are asking to match the full path of the url to redirect to home
    // instead of giving redirectTo: 'home' we give an empty string so that home means localhost:4200 and not localhost:4200/home
    {path: '**', redirectTo: '', pathMatch: 'full'}
];
