import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  // we are passing down the user to the card component from the parent member list component, so we use input
  @Input() user: User;

  constructor() { }

  ngOnInit() {
  }

}
