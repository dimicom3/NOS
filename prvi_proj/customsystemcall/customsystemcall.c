#include <linux/kernel.h>
#include <linux/sched.h>
#include <linux/syscalls.h>

void dfs(struct task_struct *task) {
	struct task_struct *child;
	struct list_head *list;
	
	printk(KERN_INFO "Name: %-20s State: %ld\tProcess ID:  %d\n", task->comm, task->__state, task->pid);
	
	list_for_each(list, &task->children){

		child = list_entry(list, struct task_struct, sibling);
		
		dfs(child);
	}

}

SYSCALL_DEFINE0(customsystemcall){

	struct task_struct *task;

	dfs(&init_task);

	return 0;

}
