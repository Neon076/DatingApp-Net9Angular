import {
  Component,
  Inject,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/members';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { AccountService } from '../../_services/account.service';
import { take } from 'rxjs';
import { HubConnectionState } from '@microsoft/signalr';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, MemberMessagesComponent, CarouselModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  // static true indicates that this memberTabs will initail when ngOnInit runs otherwise it will
  // initaill after the View is completed and that is after ngOnInit
  private messageService = inject(MessageService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  member: Member = {} as Member;
  activateTab?: TabDirective;

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
      },
    });

    this.route.paramMap.subscribe({
      next: () => this.onRouteParamsChange(),
    });
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find((x) => x.heading == heading);
      if (messageTab) messageTab.active = true;
    }
  }

  onRouteParamsChange() {
    const user = this.accountService.currentUser();
    if (!user) return;
    if (
      this.messageService.hubConnection?.state ===
        HubConnectionState.Connected &&
      this.activateTab?.heading === 'Messages'
    ) {
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user, this.member.username);
      });
    }
  }

  OnTabActivated(data: TabDirective) {
    this.activateTab = data;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: this.activateTab.heading },
      queryParamsHandling: 'merge',
    });
    if (this.activateTab.heading === 'Messages' && this.member) {
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messageService.createHubConnection(user, this.member.username);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
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
