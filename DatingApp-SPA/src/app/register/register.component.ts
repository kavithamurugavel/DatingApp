import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any; // @Input is to make use of properties from parent to child

  // To make use of child component properties in the parent (using @Output)
  // the four parts are: 1) Make use of @Output() decorator in the child component 2) emit the property that we need in the child component
  // 3) head over to the parent html and add the output property with parentheses and the corresponding parent method with $event
  // 4) Define that method in the parent component - explained again in Section 11, Lecture 113
  @Output() cancelRegister = new EventEmitter(); // @Output is to emit properties from child to parent
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      this.alertify.success('Registration Successful');
    }, error => {
      this.alertify.error('Error');
    });
  }

  cancel() {
    this.cancelRegister.emit(false); // simple example
  }

}
