import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  // we get the recipient ID from the member-detail component's User property.
  // And since member-messages is a child of member-detail we are using Input property here
  @Input() recipientID: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserID = +this.authService.decodedToken.nameid; // the plus forces nameid of type any to be a number
    // so that messages[i].recipientID === currentUserID checks passes correctly
    this.userService.getMessageThread(currentUserID, this.recipientID)
      .pipe(
        // tap allows us to do something before we subscribe to this method
        tap(messages => {
          for (let i = 0; i < messages.length; i++) {
            if (messages[i].isRead === false && messages[i].recipientID === currentUserID) {
              this.userService.markAsRead(currentUserID, messages[i].id);
            }
          }
        })
      )
      .subscribe(messages => {
        this.messages = messages;
      }, error => {
        this.alertify.error(error);
      });
  }

  sendMessage() {
    this.newMessage.recipientID = this.recipientID;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
    .subscribe((message: Message) => {
      this.messages.unshift(message); // so that the message gets added to the start of the array rather than end
      this.newMessage.content = ''; // resetting
    }, error => {
      this.alertify.error(error);
    });
  }

}
