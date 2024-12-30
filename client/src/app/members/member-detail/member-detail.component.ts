import { Component, Inject, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/members';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  // static true indicates that this memberTabs will initail when ngOnInit runs otherwise it will
  // initaill after the View is completed and that is after ngOnInit
  private messageService = inject(MessageService);
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member: Member = {} as Member;
  activateTab?: TabDirective;
  messages: Message[] = [];

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
      },
    });
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  onUpdateMessages(event:Message){
      this.messages.push(event);
  }
  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find((x) => x.heading == heading);
      if (messageTab) messageTab.active = true;
    }
  }
  OnTabActivated(data: TabDirective) {
    this.activateTab = data;

    if (
      this.activateTab.heading == 'Messages' &&
      this.messages.length == 0 &&
      this.member
    ) {
      this.messageService.getMessagesThread(this.member.username).subscribe({
        next: (messages) => (this.messages = messages),
      });
    }
  }

  // loadMember() {
  //   console.log('load Members');
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;

  //   this.memberService.getMember(username).subscribe({
  //     next: (member) => (this.member = member),
  //   });
  // }
}
