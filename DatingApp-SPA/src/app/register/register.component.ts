import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any; // @Input is to make use of properties from parent to child
  @Output() cancelRegister = new EventEmitter(); // @Output is to emit properties from child to parent
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('registration successful');
    }, error => {
      console.log('error');
    });
  }

  cancel() {
    this.cancelRegister.emit(false); // simple example
    console.log('cancelled');
  }

}
